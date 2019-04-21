using System;
using System.Collections.Generic;
using System.Linq;

namespace Eir.Common.Extensions
{
    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string input, string ending)
        {
            return input.EndsWith(ending) ? input : input + ending;
        }

        public static string Replace(this string text, string oldValue, string newValue, StringComparison comparison)
        {
            if (oldValue == null)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            while (true)
            {
                int index = text.IndexOf(oldValue, comparison);
                if (index == -1)
                {
                    return text;
                }

                text = text.Remove(index, oldValue.Length).Insert(index, newValue);
            }
        }

        public static bool Contains(this string text, string value, StringComparison comparison)
        {
            return text.IndexOf(value, comparison) >= 0;
        }

        public static string ReplaceControlChars(this string text)
        {
            return text?
                .Replace("\r", "\u2190")  // LEFTWARDS ARROW
                .Replace("\n", "\u2193")  // DOWNWARDS ARROW
                .Replace("\t", "\u2192"); // RIGHTWARDS ARROW
        }

        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Val returns all number from a string up to the first non ascii digits (0-9) occurence within a string.
        /// <para>This will return 123 from the string "0123abc" </para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Val(string input)
        {
            return int.Parse(ValAsString(input));
        }

        /// <summary>
        /// Val as string returns all number from a string up to the first non ascii digits (0-9) occurence within a string with leading zeroes if there are ones in the string.
        /// <para>This will return 00012 from a string of 00012abc</para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ValAsString(string input)
        {
            var chars = input.TrimStart().ToCharArray();
            var output = string.Empty;
            foreach (var chr in chars)
            {
                if (Char.IsDigit(chr))
                    output = output + chr;
                else
                    break;
            }
            if (output.Length < 1)
                return "0";
            return output;
        }

        public static string Left(this string str, int length)
        {
            str = str ?? string.Empty;
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            str = str ?? string.Empty;
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        /// <summary>
        /// Returns the median of an IEnumerable list of numbers.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Median(this IEnumerable<double> source)
        {
            // Create a copy of the input, and sort the copy
            var temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[count / 2 - 1];
                double b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }

        /// <summary>
        /// Returns the median of an IEnumerable list of numbers.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Median(this IEnumerable<int> source)
        {
            // Create a copy of the input, and sort the copy
            int[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (a + b) / 2m;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }
    }
}