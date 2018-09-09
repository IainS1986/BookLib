using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using BookLib.Core.Api;
using BookLib.Core.Model;
using Refit;

namespace BookLib.Core.Search
{
    public class AudibleSearchService : ISearchService
    {
        private readonly string AudibleCoverURL = "https://m.media-amazon.com/images/I/{0}._SL{1}_.jpg";

        public async Task<List<Book>> Search(string search)
        {
            var query = $"{AudibleConsts.BaseURL}/search?keywords={HttpUtility.UrlEncode(search)}".Replace("+", "%20");
            string response = await HTMLHelpers.CreateHttpRequest(new Uri(query));

            //Get results list - this returns the URLs to link to
            string regexSubstringSearch = "(?<={0})(.*)(?={1})";
            string regexProduct = string.Format(regexSubstringSearch, "href=\"\\/pd\\/", "\\?qid");
            string regexCovers = string.Format(regexSubstringSearch, "https://m.media-amazon.com/images/I/", "\\._");

            var product = Regex.Matches(response, regexProduct);
            var covers = Regex.Matches(response, regexCovers);

            // For each match, build up a simple substring from one to another
            Dictionary<Match, string> matchesToHtml = new Dictionary<Match, string>();
            Dictionary<Match, Match> productMatchToCoverMatch = new Dictionary<Match, Match>();
            for (int i = 0; i < product.Count; i++)
            {
                var firstIndex = response.IndexOf($"{product[i].ToString()}?qid", StringComparison.Ordinal);
                var length = response.Length - firstIndex;;
                if (i<product.Count - 1)
                {
                    var lastIndex = response.IndexOf($"{product[i + 1].ToString()}?qid", StringComparison.Ordinal);
                    length = lastIndex - firstIndex;
                }

                matchesToHtml.Add(product[i], response.Substring(firstIndex, length));
                productMatchToCoverMatch.Add(product[i], covers[i]);
            }

            List<Book> books = new List<Book>();
            int c = 0;
            foreach(var entry in matchesToHtml)
            {
                try
                {
                    Console.WriteLine($"Scraping for book {entry.Key}");
                    string id = GetCoverID(response, productMatchToCoverMatch[entry.Key]);

                    books.Add(new Book()
                    {
                        ProductPage = entry.Key.Value,
                        Title = GetTitle(entry.Value, $"{entry.Key}?qid"),
                        Author = $"{GetAuthor(entry.Value)} / {GetNarrator(entry.Value)}",
                        Narrator = GetNarrator(entry.Value),
                        PublishDate = GetReleaseDate(entry.Value),
                        ThumbnailURL = string.Format(AudibleCoverURL, id, 128),
                        ImageURL = string.Format(AudibleCoverURL, id, 512),
                    });

                    Console.WriteLine($"Done {++c} of {matchesToHtml.Count}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error scraping Audible for book, " + ex.Message);
                }
            }
            return books;
        }

        public async Task<bool> Synopsis(Book book)
        {
            Console.WriteLine($"Scraping for Synopsis for {book.ProductPage}");

            book.Synopsis = await GetSynopsis(book.ProductPage);

            Console.WriteLine($"Done!");

            return !string.IsNullOrEmpty(book?.Synopsis);
        }

        private async Task<string> GetSynopsis(string url)
        {
            string bookPage = $"{AudibleConsts.BaseURL}/pd/{url}";
            string bookHTML = await HTMLHelpers.CreateHttpRequest(new Uri(bookPage));

            string search = ">Summary<";
            int index = bookHTML.IndexOf(search, StringComparison.Ordinal);
            if (index == -1)
            {
                return string.Empty;
            }

            string subHTML = bookHTML.Substring(index);
            search = "bc-color-secondary\"  >";
            index = subHTML.IndexOf(search, StringComparison.Ordinal);
            if (index == -1)
            {
                return string.Empty;
            }
            index += search.Length;

            // Scan until we find </span>
            bool spanClosed = false;
            int lastIndex = index;
            // Read title until you find an open <
            while (!spanClosed)
            {
                lastIndex++;
                //Look at last 7 chars to see if its </span>
                string substring = subHTML.Substring(lastIndex - 7, 7);
                spanClosed = string.Equals(substring, "</span>");
            }

            lastIndex -= 7;
            string synopsis = subHTML.Substring(index, lastIndex - index);
            return synopsis;
        }

        private string GetCoverID(string html, Match cover)
        {
            return HTMLHelpers.Scrape(html, cover, ' ', '.');
        }

        private DateTime? GetReleaseDate(string html)
        {
            string datestr = HTMLHelpers.Scrape(html, "Release date:", ' ', '<');
            DateTime date;
            if (DateTime.TryParseExact(datestr, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
            {
                return date;
            }

            return null;
        }

        private string GetNarrator(string html)
        {
            return HTMLHelpers.Scrape(html, "href=\"/search?searchNarrator=", '>', '<');
        }

        private string GetAuthor(string html)
        {
            return HTMLHelpers.Scrape(html, "href=\"/search?searchAuthor=", '>', '<');
        }

        private string GetTitle(string html, string substring)
        {
            return HTMLHelpers.Scrape(html, substring, '>', '<');
        }
    }
}
