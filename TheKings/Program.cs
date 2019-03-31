using System;
using System.Linq;

namespace TheKings
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

        public static void ShowQuestionAndAnswer(string question, object queryResult, Func<string> getAnswer)
        {
            var answerText = queryResult != null 
                ? getAnswer() 
                : EmptyResultMessage;

            LogFunc($"{question} {answerText}\n");
        }
    }
}
