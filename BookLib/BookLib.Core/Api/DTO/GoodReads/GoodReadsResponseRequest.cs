using System;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    public class GoodReadsResponseRequest
    {
        [XmlElement(ElementName = "authentication", Type = typeof(bool))]
        public bool Authenticated { get; set; }
    }
}
