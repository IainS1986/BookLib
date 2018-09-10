using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;

namespace BookLib.Core.Search
{
    public class GraphicAudioSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            // Search query
            // Anoyingly, Graphic Audio can either return search results OR
            // returns a Series overview if it decides you're searching for a 
            // series - i.e. Search for Batman will return DC Series overview :/
            string query = $"/catalogsearch/result/?q={search}";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{GraphicAudioConsts.BaseURL}{query}"));

            string regexResultQuery = "(?=<li class=\"item col-md-3 col-smh-4\">)(.*?)(?<=<\\/li>)";
            MatchCollection products = Regex.Matches(response, regexResultQuery, RegexOptions.Singleline);

            var books = new List<Book>();
            foreach(Match prod in products)
            {
                //XML parse
                try
                {
                    Book book = new Book();

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(prod.Value);

                    XmlNode productNode = xmlDocument.SelectSingleNode(".//a[contains(@class, 'product-image')]");
                    book.ProductPage = productNode.Attributes["href"].Value;
                    book.Title = productNode.Attributes["title"].Value;

                    XmlNode imageNode = productNode.SelectSingleNode(".//img[contains(@class, 'myitems-product-image')]");
                    book.ThumbnailURL = imageNode.Attributes["src"].Value;
                    book.ImageURL = book.ThumbnailURL.Replace("small_image/265", "image/458");
                    book.Author = "GraphicAudio";

                    books.Add(book);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error parsing Product " + ex.Message);
                }
            }

            return books;
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{book.ProductPage}"));

            try
            {
                book.Synopsis = GetSynopsis(response);
                book.PublishDate = GetReleaseDate(response);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching extra book details of {book}, {ex.Message}");
            }

            return false;   
        }

        private string GetSynopsis(string html)
        {
            return HTMLHelpers.Scrape(html, "<div class=\"description\">", "</div>");
        }

        private DateTime? GetReleaseDate(string html)
        {
            string fullReleaseDate = HTMLHelpers.Scrape(html, "<div class=\"product-releasedate\">", "</div>");

            //Strip out the annoying label
            string releaseDate = fullReleaseDate.Replace("<label>Release Date:</label>", string.Empty).Trim();

            DateTime date;
            if (DateTime.TryParse(releaseDate, out date))
            {
                return date;
            }
            return null;
        }
    }
}
