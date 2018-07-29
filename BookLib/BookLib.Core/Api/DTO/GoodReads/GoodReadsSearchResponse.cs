using System;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    public class GoodReadsSearchResponse
    {
        [XmlElement(ElementName = "Request", IsNullable = true, Type = typeof(GoodReadsResponseRequest))]
        public GoodReadsResponseRequest Request { get; set; }

        [XmlElement(ElementName = "search", IsNullable = true, Type = typeof(GoodReadsSearch))]
        public GoodReadsSearch Search { get; set; }
    }
}
