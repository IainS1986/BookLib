using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

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

        public static string EncodeSearch(string search)
        {
            return HttpUtility.UrlEncode(search).Replace("+", "%20");
        }
    }
}
