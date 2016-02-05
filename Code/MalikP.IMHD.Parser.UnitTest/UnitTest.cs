using System;
using System.Collections.Generic;
using System.Linq;
using MalikP.IMHD.Parser;
using MalikP.IMHD.Parser.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;

namespace MalikP.IMHD.Parser.UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var stationProcessor = new StationProcessor("ba", "4");
            var stationsForward = stationProcessor.Process(StationDirection.Forward);
            var stationsBackward = stationProcessor.Process(StationDirection.Backward);

            var pn = StationPanBuilder.Build(RouteProcessor.Process(stationsForward, "4").ToStationSide(),
                                             RouteProcessor.Process(stationsBackward, "4").ToStationSide());
        }

        [TestMethod]
        public void ExtractHarmonogram()
        {
            for (int i = 1; i < 213; i++)
            {
                var line = i.ToString();
                var stationProcessor = new StationProcessor("ba", line);
                var stationsForward = stationProcessor.Process(StationDirection.Forward);
                var stationsBackward = stationProcessor.Process(StationDirection.Backward);

                var routes = RouteProcessor.Process(stationsForward, line);

                var routeForward = routes.FirstRegullar();
                var routeBackward = RouteProcessor.Process(stationsBackward, line).FirstRegullar();

                var harmB = ExtractHarmonogramToString(routeBackward);
                var harmF = ExtractHarmonogramToString(routeForward);

                WriteFile(routeForward, harmF);
                WriteFile(routeBackward, harmB);

            }
        }

        static void WriteFile(StationRoute route, string harmonogram)
        {
            if (route == null || string.IsNullOrEmpty(harmonogram))
                return;

            if (!Directory.Exists("Harmonograms")) Directory.CreateDirectory("Harmonograms");
            File.WriteAllText(Path.Combine("Harmonograms", string.Format("{0}-{1}-{2}.TXT", route.Line, route.FromStation.Name.Replace("-", " "), route.ToStation.Name.Replace("-", " "))), harmonogram);
        }

        string ExtractHarmonogramToString(StationRoute route)
        {
            if (route == null)
                return "";

            var builder = new StringBuilder();
            try
            {
                foreach (var board in route.RouteHarmonogram.Boards)
                {
                    foreach (var item in board.HarmonogramItems)
                    {
                        foreach (var minute in item.Minutes)
                        {
                            var line = string.Format("{0}-{1}-{2}-{3}-{4}:{5}:00", route.Line, board.Name, route.FromStation.Name.Replace("-", " "), route.ToStation.Name.Replace("-", " "), item.Hour, minute[0].ToString() + minute[1].ToString());
                            builder.AppendLine(line);
                        }
                    }
                }
            }
            catch { }

            var result = builder.ToString();
            int lastLineIndex = result.LastIndexOf(Environment.NewLine);

            if (lastLineIndex > 0)
            {
                result = result.Remove(lastLineIndex);
            }

            return result;
        }


        [TestMethod]
        public void ExtractNightHarmonogram()
        {
            var data = new int[] { 21, 29, 33, 34, 37, 44, 47, 53, 55, 56, 61, 70, 72, 74, 80, 91, 93, 95, 99 };
            foreach (var i in data)
            {
                var line = "N" + i.ToString();
                var stationProcessor = new StationProcessor("ba", line);
                var stationsForward = stationProcessor.Process(StationDirection.Forward);
                var stationsBackward = stationProcessor.Process(StationDirection.Backward);

                var routeForward = RouteProcessor.Process(stationsForward, line).FirstRegullar();
                var routeBackward = RouteProcessor.Process(stationsBackward, line).FirstRegullar();

                var harmB = ExtractHarmonogramToString(routeBackward);
                var harmF = ExtractHarmonogramToString(routeForward);

                WriteFile(routeForward, harmF);
                WriteFile(routeBackward, harmB);
            }
        }

        List<string> BoardNames = new List<string>();

        [TestMethod]
        public void GetBoardNames()
        {
            for (int i = 0; i < 213; i++)
            {
                var line = i.ToString();
                var stationProcessor = new StationProcessor("ba", line);
                var stationsForward = stationProcessor.Process(StationDirection.Forward);
                var stationsBackward = stationProcessor.Process(StationDirection.Backward);

                var routeForward = RouteProcessor.Process(stationsForward, line).FirstRegullar();
                var routeBackward = RouteProcessor.Process(stationsBackward, line).FirstRegullar();

                BoardNames.AddRange(ExtractBoardName(routeForward));
                BoardNames.AddRange(ExtractBoardName(routeBackward));
            }

            BoardNames = BoardNames.OrderBy(d => d).Distinct().ToList();
        }

        [TestMethod]
        public void GetBoardNightNames()
        {
            var data = new int[] { 21, 29, 33, 34, 37, 44, 47, 53, 55, 56, 61, 70, 72, 74, 80, 91, 93, 95, 99 };
            foreach (var i in data)
            {
                var line = "N" + i.ToString();
                var stationProcessor = new StationProcessor("ba", line);
                var stationsForward = stationProcessor.Process(StationDirection.Forward);
                var stationsBackward = stationProcessor.Process(StationDirection.Backward);

                var routeForward = RouteProcessor.Process(stationsForward, line).FirstRegullar();
                var routeBackward = RouteProcessor.Process(stationsBackward, line).FirstRegullar();

                BoardNames.AddRange(ExtractBoardName(routeForward));
                BoardNames.AddRange(ExtractBoardName(routeBackward));
            }

            BoardNames = BoardNames.OrderBy(d => d).Distinct().ToList();
        }

        public List<string> ExtractBoardName(StationRoute route)
        {
            var result = new List<string>();
            try
            {
                foreach (var board in route.RouteHarmonogram.Boards)
                {
                    foreach (var item in board.HarmonogramItems)
                    {
                        result.Add(board.Name);
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
