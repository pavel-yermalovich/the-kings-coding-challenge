// You can find this project in separate files and with unit tests here https://github.com/pavel-yermalovich/the-kings-coding-challenge

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Demo
{
    public class Program
    {
        const string KingsListUri = "http://mysafeinfo.com/api/data?list=englishmonarchs&format=json";
        const string EmptyResultMessage = "Query returned empty result";

        public static Action<string> LogFunc = Console.WriteLine;

        public static void Main()
        {
            IKingApiService kingApiService = new KingApiService(KingsListUri);
            IKingStatsService statsService = new KingStatsService(kingApiService);

            int count = statsService.GetKingsCount();
            ShowQuestionAndAnswer(
                "How many monarchs are there in the list?", count,
                () => count.ToString());

            var kings = statsService.GetKingWhoRuledLongest().ToList();
            ShowQuestionAndAnswer(
                "Which monarch ruled the longest (and for how long)?", kings,
                () => $"{string.Join(", ", kings.Select(k => k.Name))} ruled for {kings.First().Ruled.TotalYears} years");

            var houses = statsService.GetHouseWhichRuledLongest().ToList();
            ShowQuestionAndAnswer(
                "Which house ruled the longest (and for how long)?", houses,
                () => $"{string.Join(", ", houses.Select(k => k.Name))} ruled for {houses.First().YearsRuled} years");

            var names = statsService.GetMostCommonFirstName().ToList();
            ShowQuestionAndAnswer(
                "What was the most common first name?", names,
                () => $"{string.Join(", ", names.Select(k => k))}");

            Console.ReadLine();
        }

        // I use a function getAnswer instead of just string in order to evaluate answer only if a query result is not null
        public static void ShowQuestionAndAnswer(string question, object queryResult, Func<string> getAnswer)
        {
            var answerText = queryResult != null
                ? getAnswer()
                : EmptyResultMessage;

            LogFunc($"{question} {answerText}\n");
        }
    }

    public class ApiKing
    {
        public string id { get; set; }
        public string nm { get; set; }
        public string cty { get; set; }
        public string hse { get; set; }
        public string yrs { get; set; }
    }

    public class King
    {
        public King(ApiKing apiKingApi)
        {
            Id = int.Parse(apiKingApi.id);
            Name = apiKingApi.nm;
            Country = apiKingApi.cty;
            House = apiKingApi.hse;
            Ruled = YearRange.Parse(apiKingApi.yrs);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string House { get; set; }
        public YearRange Ruled { get; set; }

        public string FirstName =>
            Name.IsNotNullOrEmpty() && Name.Contains(" ")
                ? Name.Substring(0, Name.IndexOf(" ", StringComparison.Ordinal))
                : Name;
    }

    public class House
    {
        public string Name { get; set; }

        public IList<King> Kings { get; set; }

        public int YearsRuled =>
            Kings.Max(k => k.Ruled.End) - Kings.Min(k => k.Ruled.Start);
    }

    public class YearRange
    {
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

    public interface IKingApiService
    {
        IEnumerable<King> GetKings();
    }

    public class KingApiService : IKingApiService
    {
        private readonly string _apiUrl;

        public KingApiService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public IEnumerable<King> GetKings()
        {
            List<ApiKing> kings;

            using (var client = new WebClient())
            {
                var content = client.DownloadString(_apiUrl);

                var serializer = new DataContractJsonSerializer(typeof(List<ApiKing>));
                using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(content)))
                {
                    kings = (List<ApiKing>)serializer.ReadObject(memoryStream);
                }
            }

            var result = kings.Select(dto => new King(dto));
            return result;
        }
    }

    public interface IKingStatsService
    {
        int GetKingsCount();
        IEnumerable<King> GetKingWhoRuledLongest();
        IEnumerable<House> GetHouseWhichRuledLongest();
        IEnumerable<string> GetMostCommonFirstName();
    }

    // 2 max values is a real case which I discovered while writing unit tests so I decided to handle it
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
                .FirstOrDefault()?
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
                .FirstOrDefault()?
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
                .FirstOrDefault()?
                .Select(n => n.Name);
        }
    }

    public static class Extensions
    {
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
    }
}
