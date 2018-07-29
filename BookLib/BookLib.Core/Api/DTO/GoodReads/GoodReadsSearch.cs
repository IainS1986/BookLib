using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    public class GoodReadsSearch
    {
        [XmlElement(ElementName = "results-start", Type = typeof(int))]
        public int? FromIndex { get; set; }

        [XmlElement(ElementName = "results-end", Type = typeof(int))]
        public int? ToIndex { get; set; }

        [XmlElement(ElementName = "total-results", Type = typeof(int))]
        public int? TotalResult { get; set; }

        [XmlElement(ElementName = "source", Type = typeof(string))]
        public string Source { get; set; }

        [XmlElement(ElementName = "results")]
        public GoodReadsWorksList SearchResults { get; set; }
    }
}
