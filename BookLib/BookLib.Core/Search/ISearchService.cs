using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLib.Core.Model;

namespace BookLib.Core.Search
{
    public interface ISearchService
    {
        Task<List<Book>> Search(string search);
        Task<bool> Synopsis(Book book);
    }
}
