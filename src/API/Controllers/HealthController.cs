namespace API.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController: ControllerBase
{
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> GetHealth()
    {
        return Ok();
    }
}
