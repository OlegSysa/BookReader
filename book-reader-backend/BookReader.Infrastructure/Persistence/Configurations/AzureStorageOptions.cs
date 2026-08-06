using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Persistence.Configurations
{
    public class AzureStorageOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string BooksContainer { get; set; } = string.Empty;
        public string ParsedBooksContainer { get; set; } = string.Empty;
    }
}
