using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class StationRoute
    {
        protected readonly object _locker = new object();

        public string Line { get; set; }
        public Station FromStation { get; set; }
        public Station ToStation { get; set; }
        public Harmonogram _routeHarmonogram;
        public Harmonogram RouteHarmonogram
        {
            get
            {
                if (_routeHarmonogram == null)
                {
                    lock (_locker)
                    {
                        if (_routeHarmonogram == null)
                        {
                            this.WithHarmonogram();
                        }
                    }
                }

                return _routeHarmonogram;
            }
            set
            {
                _routeHarmonogram = value;
            }
        }
        protected List<Station> _viaStation;
        public List<Station> ViaStation
        {
            get
            {
                if (_viaStation == null)
                {
                    lock (_locker)
                    {
                        if (_viaStation == null)
                        {
                            _viaStation = new List<Station>();
                        }
                    }
                }

                return _viaStation;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} => {1}", FromStation.ToString(), ToStation.ToString());
        }
    }

}
