using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
            
        }

        [HttpGet]
        public object Get()
        {
            return new
            {
                status = "Healthy"
            };
        }


    }
}
