using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class HarmonogramTimeItem
    {
        protected readonly object _locker = new object();
        public string Hour { get; set; }
        protected List<string> _minutes { get; set; }
        public List<string> Minutes
        {
            get
            {
                if (_minutes == null)
                {
                    lock (_locker)
                    {
                        if (_minutes == null) _minutes = new List<string>();
                    }
                }
                return _minutes;
            }
        }

        public override string ToString() => $"Hour: {Hour}, Minutes count: {Minutes.Count}";
    }
}
