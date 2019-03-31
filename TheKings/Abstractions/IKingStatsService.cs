using System.Collections.Generic;

namespace TheKings
{
    public interface IKingStatsService
    {
        int GetKingsCount();
        IEnumerable<King> GetKingWhoRuledLongest();
        IEnumerable<House> GetHouseWhichRuledLongest();
        IEnumerable<string> GetMostCommonFirstName();
    }
}