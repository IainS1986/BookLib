using System;
using System.Threading.Tasks;
using Refit;

namespace BookLib.Core.Api
{
    public interface IAudibleAPI
    {
        [Get("/search?keywords={search}")]
        Task<string> GetSearch(string search);
    }
}
