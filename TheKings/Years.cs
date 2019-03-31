using System;
using System.Text.RegularExpressions;

namespace TheKings
{
    public class YearRange
    {
        public YearRange() { }

        public YearRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; set; }

        public int End { get; set; }

        public int TotalYears => End - Start;

        public static YearRange Parse(string years)
        {
            if (years == null)
                return null;

            Match match = Regex.Match(years, @"^(?<start>\d+)(?<delimiter>-?)(?<end>\d*)$");
            var start = match.Groups["start"].Value;
            if (string.IsNullOrEmpty(start))
                return null;

            int startYear = int.Parse(start);

            var hasHyphen = match.Groups["delimiter"].Value.IsNotNullOrEmpty();
            var end = match.Groups["end"].Value;
            int endYear = end.IsNotNullOrEmpty()
                ? int.Parse(end)
                : hasHyphen
                    ? DateTime.UtcNow.Year
                    : startYear;

            if (endYear < startYear)
                return null;

            return new YearRange(startYear, endYear);
        }
    }
}
