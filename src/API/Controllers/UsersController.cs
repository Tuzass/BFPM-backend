using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController: ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await UserService.GetUser(context, id);   
            return Ok(user);
        }
    }
}
