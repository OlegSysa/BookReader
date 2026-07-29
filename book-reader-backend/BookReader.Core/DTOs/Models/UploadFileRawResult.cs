using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
     public sealed record UploadFileRawResult(BookStatus Status, string Path);
}
