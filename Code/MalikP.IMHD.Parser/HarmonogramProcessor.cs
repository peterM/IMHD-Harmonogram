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

        public static Harmonogram GetHarmonograms(this StationRoute route)
        {
            return WithHarmonogram(route).RouteHarmonogram;
        }

        public static StationRoute WithHarmonogram(this StationRoute route)
        {
            route.RouteHarmonogram = GetHarmonogram(route);
            return route;
        }

        public static Harmonogram GetHarmonogram(StationRoute route)
        {
            var harm = new Harmonogram();

            HtmlDocument htmlDoc = GetHarmonogramHtmlDocument(route);
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
                            var harmonogramItem = new HarmonogramTimeItem()
                            {
                                Hour = allLines.Where(d => d.Attributes != null).FirstOrDefault().InnerText
                            };

                            allLines.Where(d => (d.Attributes.Count == 0 ||
                                                (d.Attributes["class"] != null && d.Attributes["class"].Value.Equals("nizkopodlazne", StringComparison.InvariantCultureIgnoreCase))))
                                     .Select(x => x.InnerText)
                                     .ToList()
                                     .ForEach(it =>
                                     {
                                         harmonogramItem.Minutes.Add(it);
                                     });

                            harmonogramBoard.HarmonogramItems.Add(harmonogramItem);
                        }
                    }

                    harmonogramBoard.Name = GetHarmonogramBoardName(htmlBoards.IndexOf(htmlBoard), htmlTables);

                    harm.Boards.Add(harmonogramBoard);
                }
            }

            return harm;
        }

        private static string GetHarmonogramBoardName(int p, IEnumerable<HtmlNode> htmlTables)
        {
            var t = htmlTables.Where(d => d.Attributes != null &&
                                     d.Attributes.Count > 0 &&
                                     d.Attributes.Contains("class") &&
                                     d.Attributes["class"].Value.Equals("cp_obsah", StringComparison.InvariantCultureIgnoreCase));
            var result =
               t.First()
                .Descendants("td")
                .Where(d => d.Attributes != null &&
                            d.Attributes.Count > 0 &&
                            d.Attributes.Contains("class") &&
                            d.Attributes["class"].Value.Equals("nazov_dna", StringComparison.InvariantCultureIgnoreCase))
                .ToList()[p].InnerText;

            return result.Replace("&nbsp;", " ");
        }

        private static List<HtmlNode> GetDepartures(HtmlNode htmlBoard)
        {
            var departureRows = htmlBoard.Descendants("tr")
                                         .Where(d => d.Attributes != null &&
                                                     d.Attributes.Contains("class") &&
                                                     d.Attributes["class"].Value.Equals("cp_odchody", StringComparison.InvariantCultureIgnoreCase))
                                         .ToList();
            return departureRows;
        }

        private static List<HtmlNode> GetHarmonogramHtmlBoards(IEnumerable<HtmlNode> tables)
        {
            return tables.Where(d => d.Attributes != null &&
                                     d.Attributes.Count > 0 &&
                                     d.Attributes.Contains("class") &&
                                     d.Attributes["class"].Value.Equals("cp_odchody_tabulka_max", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private static IEnumerable<HtmlNode> GetHarnogramHtmlTables(HtmlDocument htmlDoc)
        {
            var tables = htmlDoc.DocumentNode.Descendants("table");
            return tables;
        }

        private static HtmlDocument GetHarmonogramHtmlDocument(StationRoute route)
        {
            HtmlDocument doc = new HtmlDocument() { OptionDefaultStreamEncoding = Encoding.UTF8 };
            doc.LoadHtml(HtmlPageDownloader.DownloadHtmlPage(string.Format(HarmonogramBaseUrl, route.FromStation.Link)));
            return doc;
        }

    }
}
