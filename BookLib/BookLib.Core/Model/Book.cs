using System;
namespace BookLib.Core.Model
{
    public class Book
    {
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

        public string Synopsis
        {
            get;
            set;
        }

        public Book()
        {
        }
    }
}
