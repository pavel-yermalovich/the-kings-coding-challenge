using System;
using System.Collections.Generic;
using System.Linq;

namespace TheKings
{
    public class KingStatsService : IKingStatsService
    {
        private readonly Lazy<IEnumerable<King>> _kingsLazy;

        public KingStatsService(IKingApiService kingService)
        {
            _kingsLazy = new Lazy<IEnumerable<King>>(kingService.GetKings);
        }

        protected IEnumerable<King> Kings => _kingsLazy.Value;

        public int GetKingsCount()
        {
            return Kings.Count();
        }

        public IEnumerable<King> GetKingWhoRuledLongest()
        {
            if (!Kings.Any())
                return null;

            return Kings
                .GroupBy(k => k.Ruled.TotalYears)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                .Select(g => g)
                .OrderBy(k => k.Name);
        }

        public IEnumerable<House> GetHouseWhichRuledLongest()
        {
            if (!Kings.Any())
                return null;

            return Kings
                .GroupBy(k => k.House)
                .Select(g => new House { Name = g.Key, Kings = g.ToList() })
                .GroupBy(h => h.YearsRuled)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                .Select(h => h)
                .OrderBy(h => h.Name);
        }

        public IEnumerable<string> GetMostCommonFirstName()
        {
            if (!Kings.Any())
                return null;

            return Kings
                .GroupBy(k => k.FirstName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .GroupBy(s => s.Count)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                .Select(n => n.Name);
        }
    }
}
