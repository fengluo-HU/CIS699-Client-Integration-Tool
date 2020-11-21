using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}
