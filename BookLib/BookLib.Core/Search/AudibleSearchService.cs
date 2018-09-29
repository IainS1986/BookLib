using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;
using HtmlAgilityPack;
using Refit;

namespace BookLib.Core.Search
{
    public class AudibleSearchService : ISearchService
    {
        private readonly string AudibleCoverURL = "https://m.media-amazon.com/images/I/{0}._SL{1}_.jpg";

        public async Task<List<Book>> Search(string search)
        {
            List<Book> books = new List<Book>();
            var query = $"{AudibleConsts.BaseURL}/search?keywords={HTMLHelpers.EncodeSearch(search)}";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri(query));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Get all books (Figure with class span4-cat slide
            var products = htmlDoc.DocumentNode.Descendants("li").
                                Where(x => x.Attributes != null &&
                                           x.Attributes.Any(y => y.Name == "class" &&
                                                            y.Value.Contains("bc-list-item") &&
                                                            y.Value.Contains("productListItem"))).ToList();

            foreach(var prod in products)
            {
                try
                {
                    Book book = new Book();
                    book.Engine = SearchType.Audible;

                    var productNode = prod.Descendants("img").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-pub-block") &&
                                                                                                 y.Value.Contains("bc-image-inset-border")));
                    book.ThumbnailURL = productNode?.Attributes["src"].Value;
                    book.ImageURL = book.ThumbnailURL.Replace("SL5", "SL512");
                    book.ThumbnailURL = book.ThumbnailURL.Replace("SL5", "SL128");


                    var titleRootNode = prod.Descendants("h3").FirstOrDefault(x => x.Attributes != null &&
                                                                              x.Attributes.Any(y => y.Name == "class" &&
                                                                                               y.Value.Contains("bc-size-medium")));
                    var titleNode = titleRootNode.Descendants("a").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-link") &&
                                                                                                 y.Value.Contains("bc-color-link")) &&
                                                                                x.Attributes.Any(y => y.Name == "tabindex" &&
                                                                                                      y.Value == "0"));
                    book.Title = titleNode.InnerText;

                    var authorRootNode = prod.Descendants("li").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-list-item") &&
                                                                                                 y.Value.Contains("authorLabel")));
                    var authorNode = authorRootNode.Descendants("a").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-link") &&
                                                                                                 y.Value.Contains("bc-color-link")));
                    book.Author = authorNode.InnerText;

                    var narratorRootNode = prod.Descendants("li").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-list-item") &&
                                                                                                 y.Value.Contains("narratorLabel")));
                    var narratorNode = narratorRootNode.Descendants("a").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-link") &&
                                                                                                 y.Value.Contains("bc-color-link")));
                    book.Narrator = narratorNode.InnerText;

                    var ratingRootNode = prod.Descendants("li").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-list-item") &&
                                                                                                 y.Value.Contains("ratingsLabel")));
                    var ratingNode = ratingRootNode.Descendants("span").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-text") &&
                                                                                                 y.Value.Contains("bc-pub-offscreen")));

                    var dateRootNode = prod.Descendants("li").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-list-item") &&
                                                                                                 y.Value.Contains("releaseDateLabel")));
                    var dateNode = dateRootNode.Descendants("span").FirstOrDefault(x => x.Attributes != null &&
                                                                                x.Attributes.Any(y => y.Name == "class" &&
                                                                                                 y.Value.Contains("bc-text") &&
                                                                                                 y.Value.Contains("bc-size-small") &&
                                                                                                 y.Value.Contains("bc-color-secondary")));
                    var datestr = dateNode.InnerText.Replace("Release date:", string.Empty).Trim();
                    DateTime date;
                    if (DateTime.TryParseExact(datestr, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
                    {
                        book.PublishDate = date;
                    }

                    string page = titleNode.Attributes["href"].Value;
                    int queryIndex = page.IndexOf('?');
                    book.ProductPage = page.Substring(0, queryIndex);

                    books.Add(book);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching search - {ex.Message}");
                }
            }

            return books;
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            if (book == null ||
                string.IsNullOrEmpty(book.ProductPage))
            {
                return false;
            }

            Console.WriteLine($"Scraping for Extra Details for {book.ProductPage}");

            string bookPage = $"{AudibleConsts.BaseURL}{book.ProductPage}";
            string bookHTML = await HTMLHelpers.CreateHttpRequest(new Uri(bookPage));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bookHTML);

            try
            {
                var summaryRoot = htmlDoc.DocumentNode.Descendants("div").
                                         FirstOrDefault(x => x.Attributes != null &&
                                                             x.Attributes.Any(y => y.Name == "class" &&
                                                                                   y.Value.Contains("bc-container") &&
                                                                                   y.Value.Contains("productPublisherSummary")));
                var summary = summaryRoot.Descendants("div").
                                         FirstOrDefault(x => x.Attributes != null &&
                                                             x.Attributes.Any(y => y.Name == "class" &&
                                                                                   y.Value.Contains("bc-section") &&
                                                                                   y.Value.Contains("bc-spacing-medium")));
                book.Synopsis = summary?.InnerHtml;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching extra details for {book} - {ex.Message}");
                return false;
            }

            Console.WriteLine($"Done!");
            return true;
        }
    }
}
