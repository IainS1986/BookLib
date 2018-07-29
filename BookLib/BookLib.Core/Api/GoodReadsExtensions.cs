using System;
using BookLib.Core.Api.DTO;
using BookLib.Core.Model;

namespace BookLib.Core.Api
{
    public static class GoodReadsExtensions
    {
        public static Book ToBook(this GoodReadsWork work)
        {
            Book book = new Book();
            book.Author = work.Book?.Author?.Name;
            book.Title = work.Book?.Title;
            book.ThumbnailURL = work.Book?.SmallImageURL;
            book.ImageURL = work.Book?.ImageURL;
            return book;
        }
    }
}
