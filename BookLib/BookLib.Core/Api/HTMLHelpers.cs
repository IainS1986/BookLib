using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookLib.Core.Api
{
    public static class HTMLHelpers
    {
        /// <summary>
        /// Given a URL, loads and returns a string representing a web page's markup
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static async Task<string> CreateHttpRequest(Uri URL)
        {
            HttpWebRequest request = HttpWebRequest.Create(URL) as HttpWebRequest;
            request.Method = "GET";

            String html = "";
            try
            {
                using (WebResponse response = await request.GetResponseAsync())
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

        /// <summary>
        /// Scrapes HTML for content. Takesd HTML string as input and searches
        /// for a given string to start crawling from. From that point, it crawls
        /// until it find a given start char, and then crawls until it find the 
        /// given end char, returning the string found between start and end.
        /// </summary>
        /// <returns>The string between Start and End char found after the Search string</returns>
        /// <param name="html">The HTML string to search in</param>
        /// <param name="search">The Search string to look for to start from</param>
        /// <param name="start">The Start char to look for when crawling.</param>
        /// <param name="end">The End char to look for when crawling</param>
        public static string Scrape(string html, string search, char start, char end)
        {
            int index = html.IndexOf(search, StringComparison.Ordinal);
            if (index == -1)
            {
                return "N/A";
            }

            // From there, iterate until you find the start
            int startIndex = index + search.Length;
            while (html[startIndex] != start && start != ' ')
            {
                startIndex++;
            }

            // Don't include the char we searched to start from
            if (start != ' ')
            {
                startIndex++;
            }

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != end)
            {
                lastIndex++;
            }

            string substring = html.Substring(startIndex, (lastIndex) - startIndex);
            return substring.Trim();
        }

        public static string Scrape(string html, Match match, char start, char end)
        {
            int startIndex = match.Index;
            while (html[startIndex] != start && start != ' ')
            {
                startIndex++;
            }

            int lastIndex = startIndex;
            // Read title until you find an open <
            while (html[lastIndex] != end)
            {
                lastIndex++;
            }

            string substring = html.Substring(startIndex, (lastIndex) - startIndex);
            return substring.Trim();
        }
    }
}
