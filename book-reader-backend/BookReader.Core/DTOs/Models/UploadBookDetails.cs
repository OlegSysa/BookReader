using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public record UploadBookDetails(string FileName, long FileSize, int UserId);
    
}
