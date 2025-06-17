using Microsoft.AspNetCore.Mvc;

namespace GenApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Home controller is running");
    }
}
