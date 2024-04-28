using AuthTest.Models;
using AuthTest.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class LoginController(SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, request.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return Ok("hooray");
        }
        else if (result.RequiresTwoFactor)
        {
            return Ok("Requires two factor");
        }
        else if (result.IsLockedOut)
        {
            return Unauthorized("Locked out");
        }
        else
        {
            return Unauthorized();
        }
    }
}