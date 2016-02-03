using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class Harmonogram
    {
        protected readonly object _locker = new object();

        protected List<HarmonogramBoard> _boards;

        public List<HarmonogramBoard> Boards
        {
            get
            {
                if (_boards == null)
                {
                    lock (_locker)
                    {
                        if (_boards == null) _boards = new List<HarmonogramBoard>();
                    }
                }
                return _boards;
            }
        }
    }
}
