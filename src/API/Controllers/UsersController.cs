using Infrastructure.Database;
using Services;
using API.Requests;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController: ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
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

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        User user = new(request.UserName, request.Email);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(
                new {errors = result.Errors.Select(e => new {e.Code, e.Description})}
            );
        }

        return CreatedAtAction(
            nameof(GetUser),
            new {id = user.Id},
            new {user.Id, user.UserName, user.Email}
        );

        // TODO: add email activation at some step i guess?
    }
}
