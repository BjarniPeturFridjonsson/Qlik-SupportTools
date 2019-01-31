using System;
using System.Globalization;

namespace Eir.Common.Extensions
{
    public static class Int64Extensions
    {
        private static readonly string[] _sizes = { "B", "KB", "MB", "GB" };
        public static string AsByteSizeString(this long input)
        {
            return AsByteSizeString(input, CultureInfo.InvariantCulture);
        }
        public static string AsByteSizeString(this long input, IFormatProvider formatProvider)
        {
            double inputAsDouble = input;
            int order = 0;
            while (inputAsDouble >= 1024 && ++order < _sizes.Length)
            {
                inputAsDouble = inputAsDouble / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return $"{inputAsDouble.ToString("0.#", formatProvider)} {_sizes[Math.Min(order, _sizes.Length - 1)]}";
        }
    }
}