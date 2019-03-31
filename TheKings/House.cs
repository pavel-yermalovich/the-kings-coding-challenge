using System.Collections.Generic;
using System.Linq;

namespace TheKings
{
    public class House
    {
        public string Name { get; set; }

        public IList<King> Kings { get; set; }

        public int YearsRuled =>
            Kings.Max(k => k.Ruled.End) - Kings.Min(k => k.Ruled.Start);
    }
}
