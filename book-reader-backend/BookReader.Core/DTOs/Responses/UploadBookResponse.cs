using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Responses
{
    public class UploadBookResponse
    {
        public int Code { get; set; }
        public bool Success { get; set; }
        public BookStatus Status { get; set; }
    }
}
