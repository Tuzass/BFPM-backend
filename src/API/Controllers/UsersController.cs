using Infrastructure.Database;
using Services;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Services.Exceptions;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController: ControllerBase
{
    private readonly UserManager<User> _userManager = null!;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private UserController()
    {
        
    }

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
