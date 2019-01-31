using System.Linq;
using System.Text.RegularExpressions;

namespace Eir.Common.Search
{
    public static class RegexSupport
    {
        /// <summary>
        /// Convert a user hand-written filter string to a regex.
        /// </summary>
        /// <param name="filter">May contain the wildcard '*'.</param>
        /// <param name="allowMultiple">If true, the '|' character separate multiple searches.</param>
        /// <returns></returns>
        public static Regex CreateFromFilter(string filter, bool allowMultiple)
        {
            string[] filterWords;

            if (allowMultiple)
            {
                filterWords = filter
                    .Split('|')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            }
            else
            {
                filterWords = new[] { filter.Trim() };
            }

            if ((filterWords.Length == 0) || ((filterWords.Length == 1) && string.IsNullOrEmpty(filterWords[0])))
            {
                // Empty filter -> match all.
                return null;
            }

            if (filterWords.Any(x => x == "*"))
            {
                // A "full" wildcard is found -> match all.
                return null;
            }

            string pattern = string.Join("|", filterWords.Select(CreateRegexFilterPattern));
            return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        private static string CreateRegexFilterPattern(string filter)
        {
            filter = Regex.Escape(filter);

            if (!filter.Contains("*"))
            {
                filter = $"^{filter}$";
            }
            else
            {
                if (filter.StartsWith("*"))
                {
                    filter = filter.Substring(1);
                }
                else
                {
                    filter = "^" + filter;
                }

                if (filter.EndsWith("*"))
                {
                    filter = filter.Substring(0, filter.Length - 1);
                }
                else
                {
                    filter += "$";
                }
            }
            
            filter = filter.Replace(".", "\\.");
            filter = filter.Replace("*", ".*");
            return filter;
        }
    }
}