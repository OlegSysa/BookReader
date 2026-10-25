using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions
{
    public static class CacheExtensions
    {
        public static string BuildCacheTranslationKey(this string input, string sourceLang, string targetLang) =>
            $"translation:{sourceLang}:{targetLang}:{input.ToLower()}";
        public static string BuildCacheBookKey(int userId, int bookId) =>
            $"parsedbooks:{userId}:{bookId}";

    }
}
