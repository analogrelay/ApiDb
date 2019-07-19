using System.Text.RegularExpressions;

namespace ApiDb
{
    public static class RegexExtensions
{
    public static bool TryMatch(this Regex regex, string input, out Match match)
    {
        match = regex.Match(input);
        return match.Success;
    }

    public static bool TryMatch(this Regex regex, string input, int startat, out Match match)
    {
        match = regex.Match(input, startat);
        return match.Success;
    }

    public static bool TryMatch(this Regex regex, string input, int beginning, int length, out Match match)
    {
        match = regex.Match(input, beginning, length);
        return match.Success;
    }
}
}
