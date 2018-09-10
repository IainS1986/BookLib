using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;

namespace BookLib.Core.Search
{
    public class BigFinishSearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            string query = $"/search_results?txtSearch={search}";
            string response = await HTMLHelpers.CreateHttpRequest(new Uri($"{BigFinishConsts.BaseURL}{query}"));
            List<Book> books = new List<Book>();

            try
            {
                //Regex to get all product HTML chunks
                string regex = "(?<=<div class=\"releaseListing with-bottom-border\">)(.*?)(?<=<div class=\"clear-both\"></div>)";
                MatchCollection products = Regex.Matches(response, regex, RegexOptions.Singleline);
                foreach (Match prod in products)
                {
                    string html = $"<html><head></head><body>{prod.Value}</body></html>";
                    html = html.Replace("&pound;", "£");
                    html = html.Replace("&rsquo;", "'");
                    html = html.Replace("&lsquo;", "'");
                    html = html.Replace("&ldquo;", "\"");
                    html = html.Replace("&rdquo;", "\"");
                    html = html.Replace("&nbsp;", " ");
                    html = html.Replace("&hellip;", "...");
                    html = html.Replace(" colspan=2", string.Empty);
                    Book book = new Book();

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(html);

                    //Product Link
                    XmlNode coverReleaseNode = xmlDocument.SelectSingleNode(".//div[contains(@class, 'coverRelease')]");
                    XmlNode productURLNode = coverReleaseNode.SelectSingleNode(".//a");
                    book.ProductPage = productURLNode.Attributes["href"].Value;

                    //Covers
                    XmlNode imgNode = coverReleaseNode.SelectSingleNode(".//img");
                    book.ThumbnailURL = $"{BigFinishConsts.BaseURL}{imgNode.Attributes["src"].Value}";
                    book.ImageURL = book.ThumbnailURL.Replace("medium", "large");

                    //Release date
                    XmlNode releaseNode = coverReleaseNode.SelectSingleNode(".//p[contains(@class, 'status soon')]");
                    string releaseString = releaseNode.InnerXml.Trim();
                    //Remove the annoying formatting
                    releaseString = releaseString.Replace("Released ", string.Empty);
                    DateTime releaseDate;
                    if (DateTime.TryParse(releaseString, out releaseDate))
                    {
                        book.PublishDate = releaseDate;
                    }

                    //Series Name
                    XmlNode rangeNode = xmlDocument.SelectSingleNode(".//span[contains(@class, 'rangeName')]");
                    XmlNode rangeNameNode = rangeNode.SelectSingleNode(".//a");
                    string rangeName = rangeNameNode.InnerText.Trim();
                    rangeName = rangeName.Replace("\r", string.Empty);
                    rangeName = rangeName.Replace("\n", string.Empty);
                    rangeName = rangeName.Replace("\t", string.Empty);

                    //Book Name
                    XmlNode titleNode = xmlDocument.SelectSingleNode(".//h3[contains(@class, 'releaseHeader')]");
                    XmlNode titleNameNode = titleNode.SelectSingleNode(".//a");
                    string title = titleNameNode.InnerText.Trim();
                    title = title.Replace("\r", string.Empty);
                    title = title.Replace("\n", string.Empty);
                    title = title.Replace("\t", string.Empty);

                    book.Title = title;
                    books.Add(book);

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error parsing BigFinish search results {ex.Message}");
            }

            return books;
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            string productURL = $"{BigFinishConsts.BaseURL}{book.ProductPage}";

            try
            {
                string response = await HTMLHelpers.CreateHttpRequest(new Uri(productURL));
                book.Synopsis = HTMLHelpers.Scrape(response, "<div class=\"releaseContent\">", "<p class=\"writerDirector\">");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error parsing product page for {book}, {ex.Message}");
            }

            return false;
        }
    }
}
