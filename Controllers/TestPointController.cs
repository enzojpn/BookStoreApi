using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("test")]
public class TestPointController : ControllerBase
{
   
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Teste Ok");
    }
}
