using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public record ServiceResult<T>(T? Data, string? Error);
}
