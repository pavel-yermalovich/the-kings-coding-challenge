using System.Collections.Generic;
using System.Linq;

namespace TheKings.Tests
{
    public static class Extensions
    {
        public static House WithKing(this House house, string name, int from, int to) // to extension
        {
            house.Kings.Add(new King { Name = name, House = house.Name, Ruled = new YearRange(from, to) });
            return house;
        }
    }
}
