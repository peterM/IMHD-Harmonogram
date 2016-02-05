using HtmlAgilityPack;
using MalikP.IMHD.Parser.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public enum StationDirection
    {
        Forward,
        Backward
    }

    public class StationProcessor
    {
        public const string BaseStationsUrl = @"http://imhd.zoznam.sk/{0}/cestovny-poriadok/linka/{1}.html";
        public string Url { get; protected set; }
        public string Location { get; protected set; }
        public string Line { get; protected set; }

        public StationProcessor(string location, string line)
        {
            Location = location;
            Line = line;
            Url = string.Format(BaseStationsUrl, location, line);
        }

        public List<Station> Process(StationDirection direction)
        {
            var stationDirectionList = new List<List<Station>>();
            var stationsHtmlDocument = GetHtmlDocument();
            var stationsTable = GetStationTables(stationsHtmlDocument);

            if (stationsTable != null && stationsTable.Count > 0)
            {
                foreach (var tbl in stationsTable)
                {
                    var stationsForward = new List<Station>();
                    var rows = GetRows(tbl);
                    if (rows != null && rows.Count > 0)
                    {
                        stationDirectionList.Add(GetStations(rows));
                    }
                }
            }

            switch (direction)
            {
                case StationDirection.Backward:
                    return stationDirectionList.LastOrDefault();

                case StationDirection.Forward:
                default:
                    return stationDirectionList.FirstOrDefault();
            }
        }

        static List<HtmlNode> GetRows(HtmlNode tbl)
        {
            var rows = tbl.Descendants("tr").ToList();
            return rows;
        }

        static List<Station> GetStations(List<HtmlNode> rows)
        {
            var tempStations = new List<Station>();

            for (var i = 2; i < rows.Count - 1; i++)
            {
                var link = "";
                var d = rows[i].Descendants("td").FirstOrDefault();
                var a = d.Descendants("a");

                if (a != null && a.Count() > 0)
                {
                    link = rows[i].Descendants("td")
                                  .FirstOrDefault()
                                  .Descendants("a")
                                  .FirstOrDefault()
                                  .Attributes
                                  .First()
                                  .Value;
                }

                var stationItem = new Station
                {
                    Name = GetStationName(rows[i]),
                    Link = link,
                    IsRegullar = IsRegullarStation(rows[i])
                };

                if (!string.IsNullOrEmpty(stationItem.Name))
                {
                    tempStations.Add(stationItem);
                }
            }

            tempStations.Add(GetLastStation(rows));

            return tempStations;
        }

        static string GetStationName(HtmlNode node)
        {
            var name = "";
            try
            {
                name = node.Descendants("td")
                           .FirstOrDefault()
                           .Descendants("a")
                           .First()
                           .InnerText
                           .Replace("&nbsp;", string.Empty);
            }
            catch (Exception)
            {
                name = node.Descendants("td")
                           .FirstOrDefault()
                           .InnerText
                           .Replace("&nbsp;", string.Empty);
            }

            return name;
        }

        static bool IsRegullarStation(HtmlNode node) => node.Descendants("td")
                                                            .FirstOrDefault()
                                                            .Descendants("i")
                                                            .Count() == 0;

        static Station GetLastStation(List<HtmlNode> rows)
        {
            var lastStation = new Station
            {
                Name = rows[rows.Count - 1].Descendants("td").FirstOrDefault().InnerText.Replace("&nbsp;", string.Empty)
            };

            return lastStation;
        }

        HtmlDocument GetHtmlDocument()
        {
            var stationHtml = new HtmlDocument
            {
                OptionDefaultStreamEncoding = Encoding.UTF8
            };

            stationHtml.LoadHtml(HtmlPageDownloader.DownloadHtmlPage(Url));

            return stationHtml;
        }

        static List<HtmlNode> GetStationTables(HtmlDocument stationHtml)
        {
            return stationHtml.DocumentNode.Descendants("table")
                                           .Where(d => d.Attributes != null &&
                                                       d.Attributes.Count > 0 &&
                                                       d.Attributes["width"] != null &&
                                                       d.Attributes["width"].Value.Contains("100%") &&
                                                       d.Attributes["class"] != null &&
                                                       d.Attributes["class"].Value.Equals("tabulka", StringComparison.InvariantCultureIgnoreCase))
                                           .ToList();
        }
    }
}
