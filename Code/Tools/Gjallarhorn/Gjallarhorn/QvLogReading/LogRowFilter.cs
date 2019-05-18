using System.Collections.Generic;
using System.Linq;

namespace Gjallarhorn.QvLogReading
{
    internal class LogRowFilter
    {
        public int Index { get; }

        public List<string> Match { get; }

        public LogRowFilter(int index, IEnumerable<string> match)
        {
            Index = index;
            Match = new List<string>(match.Select(s => s.Trim()));
        }

        public bool Matches(string[] a)
        {
            if (a.Length <= Index) return false;
            return Matches(a[Index]);
        }

        public bool Matches(string s)
        {
            if (Match.Count == 0) return false;
            return Match.Any(t => t.Equals(s));
        }
    }
}
