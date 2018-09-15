using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;
using BookLib.Core.Model.Audiobookstore;
using Newtonsoft.Json;

namespace BookLib.Core.Search
{
    public class AudiobookstoreSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            //Page index is &cpndx=1
            string encodedSearch = HttpUtility.UrlEncode(search).Replace("+", "%20");
            string query = $"/search.aspx?Category=0&SearchManufacturer0&Keyword={encodedSearch}&TypeId=&SearchOption=0";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{AudiobookstoreConsts.BaseURL}{query}"));
            List<Book> books = new List<Book>();

            // Audiobook store can be a pain, if your search returns 1 result (i assume) it will instead
            // just take you straight to the product page, not a search result...

            // First attempt to process Search Results
            string regex = "(?=<figure class=\"span4-cat slide\")(.*?)(?<=</figure>)";
            MatchCollection products = Regex.Matches(response, regex, RegexOptions.Singleline);
            foreach (Match prod in products)
            {
                try
                {
                    string html = $"<html><head></head><body>{prod.Value}</body></html>";
                    html = HttpUtility.HtmlDecode(html);
                    html = html.Replace("&", "and");
                    Book book = new Book();
                    book.Engine = SearchType.Audiobookstore;

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(html);

                    XmlNode productNode = xmlDocument.SelectSingleNode(".//a[contains(@id, 'trigger')]");
                    book.ProductPage = productNode.Attributes["dataProductLink"].Value;
                    book.Title = productNode.Attributes["dataProductName"].Value;
                    book.Author = SplitOwners(productNode.Attributes["dataAuthorName"].Value);
                    book.Narrator = SplitOwners(productNode.Attributes["dataNarratorName"].Value);

                    XmlNode imageNode = productNode.SelectSingleNode(".//img[contains(@class, 'pro-img')]");
                    book.ThumbnailURL = imageNode.Attributes["src"].Value;

                    XmlNode releaseNode = xmlDocument.SelectSingleNode(".//span[contains(@class, 'titledetail-value')]");
                    string releaseString = releaseNode.InnerXml.Trim();
                    DateTime releaseDate;
                    if (DateTime.TryParse(releaseString, out releaseDate))
                    {
                        book.PublishDate = releaseDate;
                    }

                    books.Add(book);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error parsing Audiobookstore search results {ex.Message}");
                }
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

                // JSon Regex
                string jsonRegex = "(?<=<script type=\"application/ld\\+json\">)(.*?)(?=</script>)";
                Match jsonMatch = Regex.Match(response, jsonRegex, RegexOptions.Singleline);
                RootObject root = JsonConvert.DeserializeObject<RootObject>(jsonMatch.Value);
                if (root != null)
                {
                    book.Engine = SearchType.Audiobookstore;
                    book.Title = root?.mainEntity?.name;
                    book.Author = root?.mainEntity?.author?.name;
                    book.Narrator = root?.mainEntity?.readBy?.name;
                    book.Synopsis = root?.mainEntity?.description;
                    book.ThumbnailURL = root?.mainEntity?.image;
                    book.ImageURL = root?.mainEntity?.image;

                    DateTime publishDate;
                    if (DateTime.TryParse(root?.mainEntity?.datePublished, out publishDate))
                    {
                        book.PublishDate = publishDate;
                    }
                }

                // Cover image regex
                string coverRegex = "(?=<span class=\"supersize-thumb-inner\">)(.*?)(?<=</span>)";
                Match coverMatch = Regex.Match(response, coverRegex, RegexOptions.Singleline);
                string coverHtml = $"<html><head></head><body>{coverMatch.Value}</body></html>";
                coverHtml = HttpUtility.HtmlDecode(coverHtml);
                coverHtml = coverHtml.Replace("&", "and");

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(coverHtml);

                XmlNode coverNode = xmlDocument.SelectSingleNode(".//a");
                book.ImageURL = coverNode.Attributes["href"].Value;


                // Synopsis Regex
                string synopsisRegex = "(?<=<!-- Start Book Summary Section -->)(.*?)(?= <!-- End Book Summary Section -->)";
                Match synopsisMatch = Regex.Match(response, synopsisRegex, RegexOptions.Singleline);
                string synopsisHtml = $"<html><head></head><body>{synopsisMatch.Value}</body></html>";
                synopsisHtml = HttpUtility.HtmlDecode(synopsisHtml);
                synopsisHtml = synopsisHtml.Replace("&", "and");
                synopsisHtml = synopsisHtml.Replace("<BR>", "<BR></BR>");
                synopsisHtml = synopsisHtml.Replace("<br>", "<br></br>");

                XmlDocument synopsisDoc = new XmlDocument();
                synopsisDoc.LoadXml(synopsisHtml);

                XmlNode synopsisNode = synopsisDoc.SelectSingleNode(".//span[contains(@id, 'pane1')]");
                book.Synopsis = synopsisNode.InnerXml;

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
