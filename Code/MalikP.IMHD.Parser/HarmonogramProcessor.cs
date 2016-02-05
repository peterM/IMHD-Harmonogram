using HtmlAgilityPack;
using MalikP.IMHD.Parser.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public static class HarmonogramProcessor
    {
        public const string HarmonogramBaseUrl = @"http://imhd.zoznam.sk/{0}";

        public static Harmonogram GetHarmonograms(this StationRoute route) => WithHarmonogram(route).RouteHarmonogram;

        public static StationRoute WithHarmonogram(this StationRoute route)
        {
            route.RouteHarmonogram = GetHarmonogram(route);
            return route;
        }

        public static Harmonogram GetHarmonogram(StationRoute route)
        {
            var harm = new Harmonogram();

            var htmlDoc = GetHarmonogramHtmlDocument(route);
            var htmlTables = GetHarnogramHtmlTables(htmlDoc);
            var htmlBoards = GetHarmonogramHtmlBoards(htmlTables);

            if (htmlBoards != null && htmlBoards.Count > 0)
            {
                foreach (var htmlBoard in htmlBoards)
                {
                    var harmonogramBoard = new HarmonogramBoard();
                    var departures = GetDepartures(htmlBoard);

                    foreach (var row in departures)
                    {
                        var allLines = row.Descendants("td").ToList();

                        if (allLines != null && allLines.Count > 0)
                        {
                            var harmonogramItem = new HarmonogramTimeItem
                            {
                                Hour = allLines.FirstOrDefault(d => d.Attributes != null).InnerText
                            };

                            allLines.Where(d => (d.Attributes.Count == 0 ||
                                                (d.Attributes["class"] != null &&
                                                 String.Equals(d.Attributes["class"].Value, "nizkopodlazne", StringComparison.InvariantCultureIgnoreCase))))
                                    .Select(x => x.InnerText)
                                    .ToList()
                                    .ForEach(it => harmonogramItem.Minutes.Add(it));

                            harmonogramBoard.HarmonogramItems.Add(harmonogramItem);
                        }
                    }

                    harmonogramBoard.Name = GetHarmonogramBoardName(htmlBoards.IndexOf(htmlBoard), htmlTables);

                    harm.Boards.Add(harmonogramBoard);
                }
            }

            return harm;
        }

        static string GetHarmonogramBoardName(int p, IEnumerable<HtmlNode> htmlTables)
        {
            var t = htmlTables.Where(d => d.Attributes != null &&
                                          d.Attributes.Count > 0 &&
                                          d.Attributes.Contains("class") &&
                                          string.Equals(d.Attributes["class"].Value, "cp_obsah", StringComparison.InvariantCultureIgnoreCase));
            var result = t.First()
                          .Descendants("td")
                          .Where(d => d.Attributes != null &&
                                      d.Attributes.Count > 0 &&
                                      d.Attributes.Contains("class") &&
                                      string.Equals(d.Attributes["class"].Value, "nazov_dna", StringComparison.InvariantCultureIgnoreCase))
                          .ToList()[p].InnerText;

            return result.Replace("&nbsp;", " ");
        }

        static List<HtmlNode> GetDepartures(HtmlNode htmlBoard)
        {
            return htmlBoard.Descendants("tr")
                            .Where(d => d.Attributes != null &&
                                        d.Attributes.Contains("class") &&
                                        string.Equals(d.Attributes["class"].Value, "cp_odchody", StringComparison.InvariantCultureIgnoreCase))
                            .ToList();
        }

        static List<HtmlNode> GetHarmonogramHtmlBoards(IEnumerable<HtmlNode> tables)
        {
            return tables.Where(d => d.Attributes != null &&
                                     d.Attributes.Count > 0 &&
                                     d.Attributes.Contains("class") &&
                                     string.Equals(d.Attributes["class"].Value, "cp_odchody_tabulka_max", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        static IEnumerable<HtmlNode> GetHarnogramHtmlTables(HtmlDocument htmlDoc) => htmlDoc.DocumentNode
                                                                                            .Descendants("table");

        static HtmlDocument GetHarmonogramHtmlDocument(StationRoute route)
        {
            var doc = new HtmlDocument { OptionDefaultStreamEncoding = Encoding.UTF8 };
            doc.LoadHtml(HtmlPageDownloader.DownloadHtmlPage(string.Format(HarmonogramBaseUrl, route.FromStation.Link)));
            return doc;
        }
    }
}
