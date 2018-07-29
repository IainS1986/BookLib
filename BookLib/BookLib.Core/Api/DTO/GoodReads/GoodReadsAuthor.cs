using System;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    [XmlType("author")]
    public class GoodReadsAuthor
    {
        [XmlElement(ElementName = "id", Type = typeof(int))]
        public int Id { get; set; }

        [XmlElement(ElementName = "name", Type = typeof(string))]
        public string Name { get; set; }
    }
}
