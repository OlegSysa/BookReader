using BookReader.Core.Abstract.Services;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VersOne.Epub;

namespace BookReader.Core.Business.Parsers
{
    public class EpubParser : IParser
    {
        public BookExtension Extension => BookExtension.epub;

        public async Task ParseFile(string path)
        {
            var book = await EpubReader.ReadBookAsync(path);
            foreach (var chapter in book.ReadingOrder)
            {
                Console.WriteLine(chapter.Content);
            }
        }
    }
}
