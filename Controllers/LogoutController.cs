using AuthTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class LogoutController(SignInManager<User> signInManager) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok("Goodbye world");
    }
}