using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions
{
    public static class CacheExtensions
    {
        public static string BuildChacheKey(this string input, string sourceLang, string targetLang) =>
            $"translation:{sourceLang}:{targetLang}:{input.ToLower()}";

    }
}
