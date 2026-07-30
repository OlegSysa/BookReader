using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface IParser
    {
        BookExtension Extension { get; }
        Task ParseFile(string path);
    }
}
