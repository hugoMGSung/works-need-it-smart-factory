using Microsoft.AspNetCore.Mvc;

namespace HugoErpApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get() => "ERP API is alive!";
    }
}
