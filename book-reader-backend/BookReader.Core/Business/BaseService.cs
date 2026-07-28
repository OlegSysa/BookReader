using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Business
{
    public abstract class BaseService
    {
        protected readonly IConfiguration config;
        public BaseService(IConfiguration _config)
        {
            config = _config;
        }
    }
}
