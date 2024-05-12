using System;
using System.Linq;

namespace Sulfur.Extensions
{
    public static class StringExtensions
    {
        public static bool IsDigits(this string str)
        {
            var isDigits = str.All(char.IsDigit);
            return isDigits;
        }

        public static bool IsAlphabetic(this string str)
        {
            var isAlphabetic = str.All(char.IsLetter);
            return isAlphabetic;
        }

        public static string ReplaceAt(this string str, int index, string value)
        {
            var newStr = string.IsNullOrEmpty(str) ? value : string.Concat(str.AsSpan(0, index), value.AsSpan(), str.AsSpan(index + 1));
            return newStr;
        }
    }
}
