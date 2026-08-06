using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions
{
    public static class CollectionExtensions
    {

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection == null || !collection.Any();
        }
    }
}
