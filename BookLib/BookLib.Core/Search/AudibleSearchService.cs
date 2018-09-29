using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BookLib.Core.Api;
using BookLib.Core.Extensions;
using BookLib.Core.Model;
using HtmlAgilityPack;

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
            var products = htmlDoc.DocumentNode.Descendants("li").WithAttribute("class", "bc-list-item", "productListItem");

            foreach(var prod in products)
            {
                try
                {
                    Book book = new Book();
                    book.Engine = SearchType.Audible;

                    var productNode = prod.Descendants("img").FirstOrDefaultWithAttribute("class", "bc-pub-block", "bc-image-inset-border");
                    // Get the ID with regex
                    string regex = "(?<=/I/)(.*?)(?=._S)";
                    Match id = Regex.Match(productNode?.Attributes["src"].Value, regex, RegexOptions.Singleline);
            
                    book.ThumbnailURL = string.Format(AudibleCoverURL, id.Value, 128);
                    book.ImageURL = string.Format(AudibleCoverURL, id.Value, 512);


                    var titleRootNode = prod.Descendants("h3").FirstOrDefaultWithAttribute("class", "bc-size-medium");
                    var titleNode = titleRootNode.Descendants("a").FirstOrDefaultWithAttribute("class", "bc-link", "bc-color-link");
                    book.Title = titleNode.InnerText;

                    var authorRootNode = prod.Descendants("li").FirstOrDefaultWithAttribute("class", "bc-list-item", "authorLabel");
                    var authorNode = authorRootNode.Descendants("a").FirstOrDefaultWithAttribute("class", "bc-link", "bc-color-link");
                    book.Author = authorNode.InnerText;

                    var narratorRootNode = prod.Descendants("li").FirstOrDefaultWithAttribute("class", "bc-list-item", "narratorLabel");
                    var narratorNode = narratorRootNode.Descendants("a").FirstOrDefaultWithAttribute("class", "bc-link", "bc-color-link");
                    book.Narrator = narratorNode.InnerText;

                    var ratingRootNode = prod.Descendants("li").FirstOrDefaultWithAttribute("class", "bc-list-item", "ratingsLabel");
                    var ratingNode = ratingRootNode.Descendants("span").FirstOrDefaultWithAttribute("class", "bc-text", "bc-pub-offscreen");

                    var dateRootNode = prod.Descendants("li").FirstOrDefaultWithAttribute("class", "bc-list-item", "releaseDateLabel");
                    var dateNode = dateRootNode.Descendants("span").FirstOrDefaultWithAttribute("class", "bc-text", "bc-size-small", "bc-color-secondary");
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
                var summaryRoot = htmlDoc.DocumentNode.Descendants("div").FirstOrDefaultWithAttribute("class", "bc-container", "productPublisherSummary");
                var summary = summaryRoot.Descendants("div").FirstOrDefaultWithAttribute("class", "bc-section", "bc-spacing-medium");
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
