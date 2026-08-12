using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class ServiceResult<T> {
        public ServiceResult(T? data, string? error)
        {
            Data = data;
            Error = error;
        }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(Error);
    }
}
