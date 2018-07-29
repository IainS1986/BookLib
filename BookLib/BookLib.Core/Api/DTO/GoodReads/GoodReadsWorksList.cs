using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BookLib.Core.Api.DTO
{
    [XmlRoot("results")]
    [XmlInclude(typeof(GoodReadsWork))]
    public class GoodReadsWorksList
    {
        [XmlElement(ElementName = "work")]
        public List<GoodReadsWork> Results = new List<GoodReadsWork>();
    }
}
