using MalikP.IMHD.Parser.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public static class StationExtensions
    {
        public static StationSide ToStationSide(this IEnumerable<StationRoute> source)
        {
            var stationSide = new StationSide();

            source.ToList().ForEach(route => stationSide.StationRoutes.Add(route));

            return stationSide;
        }

        public static StationRoute FirstRegullar(this List<StationRoute> source)
        {
            StationRoute route = null;

            if (source == null || source.Count == 0)
                return route;

            foreach (var routeItem in source)
            {
                if (routeItem.FromStation.IsRegullar)
                {
                    route = routeItem;
                    break;
                }
            }

            return route;
        }
    }
}
