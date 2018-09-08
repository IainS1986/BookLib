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
        public async Task<List<Book>> Search(string search)
        {
            //var query = HttpUtility.UrlEncode(search).Replace("+", "%20");
            //var request = RestService.For<IAudibleAPI>(AudibleConsts.BaseURL);
            //var response = await request.GetSearch(query);

            var query = $"{AudibleConsts.BaseURL}/search?keywords={HttpUtility.UrlEncode(search)}".Replace("+", "%20");
            string response = CreateHttpRequest(new Uri(query));

            //Get results list - this returns the URLs to link to
            string regexSubstringSearch = "(?<={0})(.*)(?={1})";

            string regexURL = string.Format(regexSubstringSearch, "href=\"\\/pd\\/", "\\?qid");
            string regexCovers = string.Format(regexSubstringSearch, "https://m.media-amazon.com/images/I/", "\\._");

            var urls = Regex.Matches(response, regexURL);
            var covers = Regex.Matches(response, regexCovers);

            // For each match, build up a simple substring from one to another
            Dictionary<string, string> matchesToHtml = new Dictionary<string, string>();
            Dictionary<string, Match> coversMatches = new Dictionary<string, Match>();
            for (int i = 0; i < urls.Count; i++)
            {
                var firstIndex = response.IndexOf($"{urls[i].ToString()}?qid");
                var length = response.Length - firstIndex;;
                if (i<urls.Count - 1)
                {
                    var lastIndex = response.IndexOf($"{urls[i + 1].ToString()}?qid");
                    length = lastIndex - firstIndex;
                }

                matchesToHtml.Add(urls[i].ToString(), response.Substring(firstIndex, length));
                coversMatches.Add(urls[i].ToString(), covers[i]);
            }

            List<Book> books = new List<Book>();
            int c = 0;
            foreach(var entry in matchesToHtml)
            {
                try
                {
                    Console.WriteLine($"Scraping for book {entry.Key}");
                    string id = GetCoverID(response, coversMatches[entry.Key]);

                    books.Add(new Book()
                    {
                        Key = entry.Key,
                        Title = GetTitle(entry.Value, $"{entry.Key}?qid"),
                        Author = $"{GetAuthor(entry.Value)} / {GetNarrator(entry.Value)}",
                        Narrator = GetNarrator(entry.Value),
                        PublishDate = GetReleaseDate(entry.Value),
                        ThumbnailURL = GetCover(entry.Value, id, 128),
                        ImageURL = GetCover(entry.Value, id, 512),
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
            Console.WriteLine($"Scraping for Synopsis for {book.Key}");

            book.Synopsis = GetSynopsis(book.Key);

            Console.WriteLine($"Done!");

            return !string.IsNullOrEmpty(book?.Synopsis);
        }

        public string GetSynopsis(string url)
        {
            string bookPage = $"{AudibleConsts.BaseURL}/pd/{url}";
            string bookHTML = CreateHttpRequest(new Uri(bookPage));

            string search = ">Summary<";
            int index = bookHTML.IndexOf(search);
            if (index == -1)
            {
                return string.Empty;
            }

            string subHTML = bookHTML.Substring(index);
            search = "bc-color-secondary\"  >";
            index = subHTML.IndexOf(search);
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

        public string GetCoverID(string html, Match cover)
        {
            int index = cover.Index;
            int lastIndex = index;
            // Read title until you find an open <
            while (html[lastIndex] != '.')
            {
                lastIndex++;
            }

            string id = html.Substring(index, (lastIndex) - index).Trim();
            return id;
        }

        public string GetCover(string html, string id, int size)
        {
            string format = $"https://m.media-amazon.com/images/I/{id}._SL{size}_.jpg";
            return format;
        }

        public DateTime? GetReleaseDate(string html)
        {
            string searchString = "Release date:";
            int index = html.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return null;
            }

            int startIndex = index + searchString.Length;

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != '<')
            {
                lastIndex++;
            }

            string datestr = html.Substring(++startIndex, (lastIndex) - startIndex).Trim();
            return DateTime.ParseExact(datestr, "dd-MM-yy", CultureInfo.InvariantCulture);
        }

        public string GetNarrator(string html)
        {
            string searchString = "href=\"/search?searchNarrator=";
            int index = html.IndexOf(searchString, StringComparison.Ordinal);
            if (index == -1)
            {
                return "N/A";
            }

            // From there, iterate until you find a closing >
            int startIndex = index + searchString.Length;
            while (html[startIndex] != '>')
            {
                startIndex++;
            }

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != '<')
            {
                lastIndex++;
            }

            string narrator = html.Substring(++startIndex, (lastIndex) - startIndex);
            return narrator;
        }

        public string GetAuthor(string html)
        {
            string searchString = "href=\"/search?searchAuthor=";
            int index = html.IndexOf(searchString, StringComparison.Ordinal);
            if (index == -1)
            {
                return "N/A";
            }

            // From there, iterate until you find a closing >
            int startIndex = index + searchString.Length;
            while (html[startIndex] != '>')
            {
                startIndex++;
            }

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != '<')
            {
                lastIndex++;
            }

            string author = html.Substring(++startIndex, (lastIndex) - startIndex);
            return author;
        }

        public string GetTitle(string html, string substring)
        {
            // First scan for the substring index
            int index = html.IndexOf(substring, StringComparison.Ordinal);

            // From there, iterate until you find a closing >
            int startIndex = index + substring.Length;
            while(html[startIndex] != '>')
            {
                startIndex++;
            }

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != '<')
            {
                lastIndex++;
            }

            return html.Substring(++startIndex, (lastIndex) - startIndex);
        }

        /// <summary>
        /// Given a URL, loads and returns a string representing a web page's markup
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string CreateHttpRequest(Uri URL)
        {
            HttpWebRequest request = HttpWebRequest.Create(URL) as HttpWebRequest;
            request.Method = "GET";
            //request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            //request.Host = "www.audible.co.uk";

            String html = "";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            html = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching URL {URL}, {e.Message}");
            }

            return html;
        }
    }
}
