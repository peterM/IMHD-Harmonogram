using MalikP.IMHD.Parser.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public class StationPanBuilder
    {
        public static StationPan Build(params StationSide[] sides)
        {
            var pan = new StationPan();

            foreach (var side in sides)
            {
                if (side != null && side.StationRoutes.Count > 0)
                    pan.StationSides.Add(side);
            }

            return pan;
        }
    }
}
