using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class StationPan
    {
        protected readonly object _locker = new object();
        protected List<StationSide> _stationSides { get; set; }

        public List<StationSide> StationSides
        {
            get
            {
                if (_stationSides == null)
                {
                    lock (_locker)
                    {
                        if (_stationSides == null)
                        {
                            _stationSides = new List<StationSide>();
                        }
                    }
                }

                return _stationSides;
            }
        }
    }
}
