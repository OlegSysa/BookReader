using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Business
{
    public abstract class BaseService<T>
    {
        protected readonly IConfiguration _config;
        protected readonly ILogger<T> _logger;
        public BaseService(IConfiguration config, ILogger<T> logger)
        {
            _config = config;
            _logger = logger;
        }
    }
}
