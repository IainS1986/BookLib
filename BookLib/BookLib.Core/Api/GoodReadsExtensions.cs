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
            book.Rating = work.Rating;

            //Date
            if (!string.IsNullOrEmpty(work.Year))
            {
                int year = 0;
                int month = 1;
                int day = 1;

                bool yearParse = int.TryParse(work.Year, out year);
                bool monthParse = int.TryParse(work.Month, out month);
                bool dayParse = int.TryParse(work.Day, out day);

                if(yearParse)
                {
                    book.PublishDate = new DateTime(year, 
                                                    (monthParse) ? month : 1, 
                                                    (dayParse) ? day : 1);
                }
            }

            return book;
        }
    }
}
