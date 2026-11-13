using Infrastructure.Database;
using Domain.Entities;
using Services;
using Services.Exceptions;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using API.Responses;

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
            id = user.Id,
            username = user.UserName,
            email = user.Email
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await UserService.GetUserByID(context, id);
            UserResponse userResponse = new(user.Id, user.UserName!, user.Email!);
            return Ok(userResponse);
        }
    }

    [Authorize]
    [HttpGet("{id:int}/projects")]
    public async Task<IActionResult> GetUserProjectIDs(int id)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var projects = await DomainService.GetProjectsByUser(context, id);
            
            List<int> projectIDs = [];
            foreach (var project in projects)
            {
                projectIDs.Add(project.ID);
            }

            return Ok(new {project_ids = projectIDs});
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        if (await UserService.IsUsernameDuplicate(request.UserName))
        {
            RegisterConflictResponse registerConflictResponse = new(
                "This username is already taken.",
                "username"
            );

            return Conflict(registerConflictResponse);
        }

        if (await UserService.IsEmailDuplicate(request.Email))
        {
            RegisterConflictResponse registerConflictResponse = new(
                "This email address is already registered.",
                "email"
            );

            return Conflict(registerConflictResponse);
        }

        User user = new(request.UserName, request.Email);
        var result = await _userManager.CreateAsync(user, request.Password);
        UserResponse userResponse = new(user.Id, user.UserName!, user.Email!);

        if (!result.Succeeded)
        {
            return BadRequest(
                new {errors = result.Errors.Select(e => new {e.Code, e.Description})}
            );
        }

        return CreatedAtAction(
            nameof(GetUser),
            new {id = user.Id},
            userResponse
        );

        // TODO: add email activation at some step i guess?
    }
}
