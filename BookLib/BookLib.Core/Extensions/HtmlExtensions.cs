
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace BookLib.Core.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlNode FirstOrDefaultWithAttribute(this IEnumerable<HtmlNode> nodes, string name, params string[] values)
        {
            return nodes.FirstOrDefault(x => x.Attributes != null &&
                                             x.Attributes.Any(y => y.Name == name &&
                                                              values.All(z => y.Value.Contains(z))));
        }

        public static IEnumerable<HtmlNode> WithAttribute(this IEnumerable<HtmlNode> nodes, string name, params string[] values)
        {
            return nodes.Where(x => x.Attributes != null &&
                                    x.Attributes.Any(y => y.Name == name &&
                                                          values.All(z => y.Value.Contains(z))));
        }
    }
}
