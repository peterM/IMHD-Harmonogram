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

        public override string ToString() => $"Name: {Name}";

        public override bool Equals(object obj)
        {
            var o = obj as Station;
            if (o == null)
                return false;

            return string.Equals(o.Name, Name, StringComparison.InvariantCultureIgnoreCase); //&&
        }

        public override int GetHashCode() => Name.ToUpper().GetHashCode() + Link.ToUpper().GetHashCode();
    }
}