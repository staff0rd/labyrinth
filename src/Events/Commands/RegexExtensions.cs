using System.Linq;
using System.Text.RegularExpressions;

namespace Events
{
    public static class RegexExtensions
    {
        public static string[] GetGroupMatches(this MatchCollection matches)
        {
            if (matches.Count == 0)
                return new string[0];
            return matches 
                .Cast<Match>()
                .Single().Groups
                .Cast<System.Text.RegularExpressions.Group>()
                .Skip(1)
                .Select(g => g.Value)
                .ToArray();
        }
    }
}