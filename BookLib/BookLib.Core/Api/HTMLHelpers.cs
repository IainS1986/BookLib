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
            //request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.92 Safari/537.36";

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

        public static string Scrape(string html, string searchString, string endTag)
        {
            int index = html.IndexOf(searchString);
            if (index == -1)
            {
                return "N/A";
            }

            //Scan until />
            bool closedTag = false;
            index += searchString.Length;
            int lastIndex = index;
            while (!closedTag)
            {
                lastIndex++;
                //Look for endTag
                string tagCheck = html.Substring(lastIndex - endTag.Length, endTag.Length);
                closedTag = string.Equals(tagCheck, endTag);
            }
            lastIndex -= endTag.Length;

            return html.Substring(index, lastIndex - index);
        }
    }
}
