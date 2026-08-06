using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions
{
    public static class CacheExtensions
    {
        public static string BuildChacheTranslationKey(this string input, string sourceLang, string targetLang) =>
            $"translation:{sourceLang}:{targetLang}:{input.ToLower()}";
        public static string BuildChacheChapterKey(int bookId, int chapterIndex) =>
            $"chapters:{bookId}:{chapterIndex}";

    }
}
