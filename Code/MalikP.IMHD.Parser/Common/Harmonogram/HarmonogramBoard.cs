using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class HarmonogramBoard
    {
        protected readonly object _locker = new object();

        protected List<HarmonogramTimeItem> _items;
        public string Name { get; set; }

        public List<HarmonogramTimeItem> HarmonogramItems
        {
            get
            {
                if (_items == null)
                {
                    lock (_locker)
                    {
                        if (_items == null) _items = new List<HarmonogramTimeItem>();
                    }
                }
                return _items;
            }
        }
    }
}
