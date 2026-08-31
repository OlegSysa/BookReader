using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Business
{
    public abstract class BaseService<T>
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IConfiguration _config;
        protected readonly ILogger<T> _logger;
        public BaseService(IConfiguration config, ILogger<T> logger, IHttpContextAccessor accessor)
        {
            _config = config;
            _logger = logger;
            _httpContextAccessor = accessor;
        }

        /// <summary>
        /// Only for http services (not consumers)
        /// </summary>
        protected string? CorrelationId =>
            _httpContextAccessor.HttpContext?
                .Request.Headers["X-Correlation-ID"]
                .FirstOrDefault();
    }
}
