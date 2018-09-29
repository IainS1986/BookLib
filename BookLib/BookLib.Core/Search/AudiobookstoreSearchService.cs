using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;
using BookLib.Core.Model.Audiobookstore;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace BookLib.Core.Search
{
    public class AudiobookstoreSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            //Page index is &cpndx=1
            List<Book> books = new List<Book>();
            string query = $"/search.aspx?Category=0&SearchManufacturer0&Keyword={HTMLHelpers.EncodeSearch(search)}&TypeId=&SearchOption=0";
            var response = await HTMLHelpers.CreateHttpRequest(new Uri($"{AudiobookstoreConsts.BaseURL}{query}"));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Get all books (Figure with class span4-cat slide
            var products = htmlDoc.DocumentNode.Descendants("figure").
                                Where(x => x.Attributes != null &&
                                           x.Attributes.Any(y => y.Name == "class" &&
                                                            y.Value == "span4-cat slide")).ToList();

            foreach(var prod in products)
            {
                Book book = new Book();
                book.Engine = SearchType.Audiobookstore;

                var productNode = prod.Descendants("a").FirstOrDefault(x => x.Attributes != null &&
                                                                            x.Attributes.Any(y => y.Name == "id" &&
                                                                                             y.Value == "trigger"));
                
                book.ProductPage = productNode?.Attributes["dataProductLink"].Value;
                book.Title = productNode?.Attributes["dataProductName"].Value;
                book.Author = SplitOwners(productNode?.Attributes["dataAuthorName"].Value);
                book.Narrator = SplitOwners(productNode?.Attributes["dataNarratorName"].Value);
                book.ThumbnailURL = productNode?.Attributes["dataImage"].Value;
                decimal rating;
                if (decimal.TryParse(productNode?.Attributes["dataRating"].Value, out rating))
                    book.Rating = rating;
 
                books.Add(book);
            }

            // IF books are still empty, try parsing the search result likeits a product page...
            if (books.Count == 0)
            {
                var singleResult = ParseProductHTML(response);
                if (singleResult != null)
                {
                    books.Add(singleResult);
                }
            }

            return books;
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            if (!string.IsNullOrEmpty(book.ThumbnailURL) &&
                !string.IsNullOrEmpty(book.Synopsis))
            {
                return true;
            }

            string productURL = $"{book.ProductPage}";

            try
            {
                string response = await HTMLHelpers.CreateHttpRequest(new Uri(productURL));

                var bk = ParseProductHTML(response);
                if (bk != null)
                {
                    book.ImageURL = bk.ImageURL;
                    book.Synopsis = bk.Synopsis;
                }

                return bk != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing product page for {book}, {ex.Message}");
            }

            return false;
        }

        private Book ParseProductHTML(string response)
        {
            try
            {
                Book book = new Book();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var jsonNode = htmlDoc.DocumentNode.Descendants("script").
                                      FirstOrDefault(x => x.Attributes != null &&
                                                          x.Attributes.Any(y => y.Name == "type" &&
                                                                                y.Value == "application/ld+json"));
                RootObject root = JsonConvert.DeserializeObject<RootObject>(jsonNode?.InnerHtml);
                if (root != null)
                {
                    book.Engine = SearchType.Audiobookstore;
                    book.Title = root?.mainEntity?.name;
                    book.Author = root?.mainEntity?.author?.name;
                    book.Narrator = root?.mainEntity?.readBy?.name;
                    book.Synopsis = root?.mainEntity?.description;
                    book.ThumbnailURL = root?.mainEntity?.image;

                    DateTime publishDate;
                    if (DateTime.TryParse(root?.mainEntity?.datePublished, out publishDate))
                    {
                        book.PublishDate = publishDate;
                    }
                }

                // Bigger cover image
                var coverNode = htmlDoc.DocumentNode.Descendants("img").
                                        FirstOrDefault(x => x.Attributes != null &&
                                                            x.Attributes.Any(y => y.Name == "id" &&
                                                                                  y.Value == "imageAudiobookCoverArt"));

                book.ImageURL = coverNode?.Attributes["src"].Value;

                return book;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing product page, {ex.Message}");
            }

            return null;
        }

        private string SplitOwners(string owners)
        {
            var splits = owners.Split('|').Where(x => !string.IsNullOrEmpty(x));
            return string.Join(", ", splits);
        }
    }
}
