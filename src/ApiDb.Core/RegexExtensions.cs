using System.Text.RegularExpressions;

namespace ApiDb
{
    public static class RegexExtensions
    {
        public static bool IsMatch(this Regex regex, string input, out Match match)
        {
            if (regex is null)
            {
                throw new System.ArgumentNullException(nameof(regex));
            }

            if (string.IsNullOrEmpty(input))
            {
                throw new System.ArgumentException("message", nameof(input));
            }

            match = regex.Match(input);
            return match.Success;
        }
    }
}
