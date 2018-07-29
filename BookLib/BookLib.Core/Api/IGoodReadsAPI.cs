using System;
using System.Threading.Tasks;
using BookLib.Core.Api.DTO;
using Refit;

namespace BookLib.Core.Api
{
    public interface IGoodReadsAPI
    {
        [Get("/search/index.xml?key={key}&q={search}")]
        Task<string> GetSearch(string key, string search);
    }
}
