using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var product = Regex.Matches(response, regexResultQuery, RegexOptions.Singleline);

            return new List<Book>();
        }

        public async Task<bool> GetExtraDetails(Book book)
        {
            await Task.CompletedTask;

            return false;   
        }
    }
}
