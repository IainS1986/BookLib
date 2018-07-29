using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using BookLib.Core.Api;
using BookLib.Core.Api.DTO;
using BookLib.Core.Model;
using Refit;

namespace BookLib.Core.Search
{
    public class SearchService : ISearchService
    {
        public async Task<List<Book>> Search(string search)
        {
            var goodReadsSearch = RestService.For<IGoodReadsAPI>(GoodReadsConsts.BaseURL);
            var response = await goodReadsSearch.GetSearch(APIKeys.GoodReads, search);
            var result = ParseResponse<GoodReadsSearchResponse>(response, GoodReadsConsts.XMLRootName);

            return result?.Search?.SearchResults?.Results?.Select(x => x.ToBook()).ToList();
        }

        private T ParseResponse<T>(string result, string rootName) where T : class
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = rootName;
            xRoot.IsNullable = true;
            XmlSerializer xs = new XmlSerializer(typeof(T), xRoot);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            return xs.Deserialize(ms) as T;
        }
    }
}
