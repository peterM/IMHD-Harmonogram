using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser.Common
{
    public class Station
    {
        public string Name { get; set; }
        public bool IsRegullar { get; set; }
        public string Link { get; set; }

        public Station()
        {
            IsRegullar = true;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", Name);
        }

        public override bool Equals(object obj)
        {
            Station o = obj as Station;
            if (o == null)
                return false;

            if (o.Name.ToUpper().Equals(Name.ToUpper())) //&&

                //o.Link.ToUpper().Equals(Link.ToUpper(), StringComparison.InvariantCultureIgnoreCase))
                return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return Name.ToUpper().GetHashCode() + Link.ToUpper().GetHashCode();
        }
        //public string From { get; set; }
        //public string To { get; set; }
    }
}