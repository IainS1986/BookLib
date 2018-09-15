using System;
using BookLib.Core.Search;

namespace BookLib.Core.Model
{
    public class Book
    {
        public SearchType Engine
        {
            get;
            set;
        }

        public string ProductPage
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Narrator
        {
            get;
            set;
        }

        public string ThumbnailURL
        {
            get;
            set;
        }

        public string ImageURL
        {
            get;
            set;
        }

        public decimal Rating
        {
            get;
            set;
        }

        public DateTime? PublishDate
        {
            get;
            set;
        }

        public string PublishDateFormatted
        {
            get
            {
                if (PublishDate.HasValue == false)
                    return string.Empty;

                return PublishDate.Value.ToString("MMMM yyyy");
            }
        }

        public string Synopsis
        {
            get;
            set;
        }

        public string Genre
        {
            get;
            set;
        }

        public Book()
        {
        }
    }
}
