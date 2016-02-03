using MalikP.IMHD.Parser.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public static class RouteProcessor
    {
        public static List<StationRoute> Process(List<Station> stations, string line)
        {
            if (stations == null || stations.Count == 0)
                return null;
            return CreateRoutes(stations, line);
        }

        private static List<StationRoute> CreateRoutes(List<Station> tempStations, string line)
        {
            var routes = new List<StationRoute>();

            foreach (var station in tempStations)
            {
                StationRoute route = new StationRoute()
                {
                    FromStation = station,
                    ToStation = tempStations.Last(),
                    Line = line
                };

                for (int i = tempStations.IndexOf(station) + 1; i < tempStations.Count - 1; i++)
                {
                    route.ViaStation.Add(tempStations[i]);
                }

                routes.Add(route);
            }

            routes.Remove(routes.Last());

            return routes;
        }
    }
}