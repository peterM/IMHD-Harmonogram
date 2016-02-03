using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class StationSide
    {
        protected readonly object _locker = new object();
        protected List<StationRoute> _stationRoutes { get; set; }

        public List<StationRoute> StationRoutes
        {
            get
            {
                if (_stationRoutes == null)
                {
                    lock (_locker)
                    {
                        if (_stationRoutes == null)
                        {
                            _stationRoutes = new List<StationRoute>();
                        }
                    }
                }

                return _stationRoutes;
            }
        }
    }
}
