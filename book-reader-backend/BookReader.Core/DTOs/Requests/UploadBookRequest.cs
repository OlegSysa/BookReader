using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Requests
{
    public class UploadBookRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
