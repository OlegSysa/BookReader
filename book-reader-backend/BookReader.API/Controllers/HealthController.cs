using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : BaseAPIController
    {
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
