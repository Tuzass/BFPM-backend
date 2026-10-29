using Infrastructure.Database;
using Domain.Entities;
using Services;
using Services.Exceptions;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await UserService.GetUserByID(context, id);   

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        if (await UserService.IsUsernameDuplicate(request.UserName))
        {
            throw new DuplicateUsernameException();
        }

        if (await UserService.IsEmailDuplicate(request.Email))
        {
            throw new DuplicateEmailException();
        }

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
