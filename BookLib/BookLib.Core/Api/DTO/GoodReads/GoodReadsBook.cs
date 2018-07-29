using System;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    [XmlType("best_book")]
    public class GoodReadsBook
    {
        [XmlElement(ElementName = "id", Type = typeof(int))]
        public int Id { get; set; }

        [XmlElement(ElementName = "title", Type = typeof(string))]
        public string Title { get; set; }

        [XmlElement(ElementName = "author", Type = typeof(GoodReadsAuthor))]
        public GoodReadsAuthor Author { get; set; }

        [XmlElement(ElementName = "image_url", Type = typeof(string))]
        public string ImageURL { get; set; }

        [XmlElement(ElementName = "small_image_url", Type = typeof(string))]
        public string SmallImageURL { get; set; }
    }
}
