using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    [XmlType("work")]
    public class GoodReadsWork
    {
        [XmlElement(ElementName = "id", Type = typeof(int))]
        public int Id { get; set; }

        [XmlElement(ElementName = "books_count", Type = typeof(int))]
        public int BooksCount { get; set; }

        [XmlElement(ElementName = "ratings_count", Type = typeof(int))]
        public int RatingsCount { get; set; }

        [XmlElement(ElementName = "text_reviews_count", Type = typeof(int))]
        public int ReviewsCount { get; set; }

        [XmlElement(ElementName = "original_publication_year", Type = typeof(string))]
        public string Year { get; set; }

        [XmlElement(ElementName = "original_publication_month", Type = typeof(string))]
        public string Month { get; set; }

        [XmlElement(ElementName = "original_publication_day", Type = typeof(string))]
        public string Day { get; set; }

        [XmlElement(ElementName = "average_rating", Type = typeof(decimal))]
        public decimal Rating { get; set; }

        [XmlElement(ElementName = "best_book", Type = typeof(GoodReadsBook))]
        public GoodReadsBook Book { get; set; }

    }
}
