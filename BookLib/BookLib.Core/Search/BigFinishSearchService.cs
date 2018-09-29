using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLib.Core.Api;
using BookLib.Core.Extensions;
using BookLib.Core.Model;
using HtmlAgilityPack;

namespace BookLib.Core.Search
{
    public class BigFinishSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            string query = $"/search_results?txtSearch={HTMLHelpers.EncodeSearch(search)}";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{BigFinishConsts.BaseURL}{query}"));
            List<Book> books = new List<Book>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Get all books
            var products = htmlDoc.DocumentNode.Descendants("div").WithAttribute("class", "releaseListing with-bottom-border");

            foreach(var prod in products)
            {
                try
                {
                    Book book = new Book();
                    book.Engine = SearchType.BigFinish;

                    // Product Link
                    var prodLinkNode = prod.Descendants("div").FirstOrDefaultWithAttribute("class", "coverRelease");

                    // Covers
                    var imgLinkNode = prodLinkNode.Descendants("img").FirstOrDefault();
                    book.ThumbnailURL = $"{BigFinishConsts.BaseURL}{imgLinkNode?.Attributes["src"].Value}";
                    book.ImageURL = book.ThumbnailURL.Replace("medium", "large");

                    // Release Date
                    var pLinkNode = prodLinkNode.Descendants("p").FirstOrDefault();
                    //Remove the annoying formatting
                    var releaseString = pLinkNode.InnerHtml.Trim();
                    releaseString = releaseString.Replace("Released ", string.Empty);
                    DateTime releaseDate;
                    if (DateTime.TryParse(releaseString, out releaseDate))
                    {
                        book.PublishDate = releaseDate;
                    }

                    // Series Name
                    var seriesNode = prod.Descendants("span").FirstOrDefaultWithAttribute("class", "rangeName");
                    var aSeriesNode = seriesNode?.Descendants("a").FirstOrDefault();
                    string seriesName = aSeriesNode?.InnerText.Trim().
                                                   Replace("\r", string.Empty).
                                                   Replace("\n", string.Empty).
                                                   Replace("\t", string.Empty);
                    book.Series = seriesName;
                    book.SeriesPage = aSeriesNode?.Attributes["href"].Value;

                    // Book name
                    var bookNode = prod.Descendants("h3").FirstOrDefaultWithAttribute("class", "releaseHeader");
                    var aBookNode = bookNode?.Descendants("a").FirstOrDefault();
                    string bookName = aBookNode?.InnerText.Trim().
                                                   Replace("\r", string.Empty).
                                                   Replace("\n", string.Empty).
                                                   Replace("\t", string.Empty);
                    book.Title = bookName;
                    book.ProductPage = aBookNode?.Attributes["href"].Value;

                    books.Add(book);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing search for {search}, {ex.Message}");
                }
            }
            return books;
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            string productURL = $"{BigFinishConsts.BaseURL}{book.ProductPage}";

            try
            {
                string response = await HTMLHelpers.CreateHttpRequest(new Uri(productURL));
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var synopsis = htmlDoc.DocumentNode.Descendants("div").FirstOrDefaultWithAttribute("class", "releaseContent");
                book.Synopsis = synopsis?.InnerHtml;
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error parsing product page for {book}, {ex.Message}");
            }

            return false;
        }
    }
}
