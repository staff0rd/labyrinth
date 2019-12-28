using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Browser.LinkedIn
{
    public class RelativeTimeConverter
    {
        public DateTimeOffset Convert(string connected)
        {
            var pattern = @"Connected (\d{1,2}) ((?:day|week|month|year))s? ago";
            
            var matches = Regex.Matches(connected, pattern)
                .Cast<Match>()
                .Single() .Groups
                .Cast<Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();

            if (matches.Count() != 2)
                throw new ArgumentException($"Unknown relative time: {connected}");

            var count = int.Parse(matches[0]);
            var period = matches[1];

            switch (period) {
                case "day": return DateTimeOffset.Now.AddDays(-count).Date;
                case "week": return DateTimeOffset.Now.AddDays(-count*7).Date;
                case "month": return DateTimeOffset.Now.AddMonths(-count).Date;
                case "year": return DateTimeOffset.Now.AddYears(-count).Date;
                default: throw new ArgumentException($"Unknown period: {period}");
            }
        }
    }
}