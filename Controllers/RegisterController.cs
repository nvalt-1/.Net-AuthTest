using AuthTest.Models;
using AuthTest.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class RegisterController(UserManager<User> userManager) : Controller
{
    [HttpPost]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        var user = new User { UserName = request.Username, };
        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return Ok($"Registered {request.Username}");
        }

        return BadRequest(result.Errors);
    }
}