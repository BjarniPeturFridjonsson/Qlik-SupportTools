using System;
using System.Collections.Generic;

namespace FreyrViewer.Services
{

    public class ProcessFixedWidthOutput
    {
        public class Header
        {
            public string HeaderName { get; set; }
            public int Start { get; set; }
            public int Stop { get; set; }
        }

        public List<Header> Headers { get; set; }
        public List<string[]> Lines { get; set; }

        public ProcessFixedWidthOutput ProcessFileData(string filecontent)
        {
            if (string.IsNullOrEmpty(filecontent)) return null;
            var s2 = filecontent.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var header = s2[1];

            bool hasFoundFirstLetter = false;
            int spaceCount = 0;
            string chars = "";
            int start = 0;
            int i = 0;

            Headers = new List<Header>();
            foreach (var letter in header)
            {

                if (letter != ' ')
                {

                    if (spaceCount > 1 && hasFoundFirstLetter)
                    {
                        if (!string.IsNullOrWhiteSpace(chars))
                        {
                            Headers.Add(new Header
                            {
                                HeaderName = chars.Trim(),
                                Start = start,
                                Stop = i,
                            });
                            chars = "";

                        }
                        start = i;
                    }
                    hasFoundFirstLetter = true;
                    spaceCount = 0;
                    chars += letter;
                }
                else
                {
                    spaceCount++;
                    chars += letter;
                }
                i++;
            }
            Headers.Add(new Header
            {
                HeaderName = chars.Trim(),
                Start = start,
                Stop = -1,
            });

            Lines = new List<string[]>();
            for (var ii = 2; ii < s2.Length; ii++)
            {
                var iii = 0;
                var a = new string[Headers.Count];
                foreach (Header p in Headers)
                {
                    if ((s2[ii].Length >= p.Stop || p.Stop == -1) && p.Start < s2[ii].Length)
                    {
                        if (p.Stop > 0)
                            a[iii] = s2[ii].Substring(p.Start, p.Stop - p.Start);
                        else
                        {
                            a[iii] = s2[ii].Substring(p.Start);
                        }
                    }

                    iii++;
                }

                Lines.Add(a);
            }
            return this;
        }
    }
}
