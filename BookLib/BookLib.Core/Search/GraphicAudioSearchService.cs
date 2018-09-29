using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Extensions;
using BookLib.Core.Model;
using HtmlAgilityPack;

namespace BookLib.Core.Search
{
    public class GraphicAudioSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            List<Book> books = new List<Book>();
            string query = $"/catalogsearch/result/?q={HTMLHelpers.EncodeSearch(search)}";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{GraphicAudioConsts.BaseURL}{query}"));
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Get all books (Figure with class span4-cat slide
            var products = htmlDoc.DocumentNode.Descendants("li").WithAttribute("class", "item col-md-3 col-smh-4");

            foreach(var prod in products)
            {
                try
                {
                    Book book = new Book();
                    book.Engine = SearchType.GraphicAudio;

                    var productNode = prod.Descendants("a").FirstOrDefaultWithAttribute("class", "product-image");
                    book.ProductPage = productNode?.Attributes["href"].Value;
                    book.Title = productNode?.Attributes["title"].Value;

                    var imageNode = prod.Descendants("img").FirstOrDefaultWithAttribute("class", "myitems-product-image");
                    book.ThumbnailURL = imageNode?.Attributes["src"].Value;
                    book.ImageURL = book.ThumbnailURL.Replace("small_image/265", "image/458");
                    book.Author = "GraphicAudio";

                    var seriesNode = prod.Descendants("div").FirstOrDefaultWithAttribute("class", "series-name");
                    book.Series = seriesNode?.InnerText;

                    books.Add(book);
                }
                catch (Exception ex)
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
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var synopsisNode = htmlDoc.DocumentNode.Descendants("div").FirstOrDefaultWithAttribute("class", "description");
                book.Synopsis = synopsisNode?.InnerHtml;

                var releaseNode = htmlDoc.DocumentNode.Descendants("div").FirstOrDefaultWithAttribute("class", "product-releasedate");
                string fullReleaseDate = releaseNode?.InnerHtml;
                string releaseDate = fullReleaseDate.Replace("<label>Release Date:</label>", string.Empty).Trim();
                DateTime date;
                if (DateTime.TryParse(releaseDate, out date))
                {
                    book.PublishDate = date;
                }

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching extra book details of {book}, {ex.Message}");
            }

            return false;   
        }
    }
}
