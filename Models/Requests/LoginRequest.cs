using System.ComponentModel.DataAnnotations;

namespace AuthTest.Models.Requests;

public class LoginRequest
{
    [Required] public string Username { get; set; } = default!;
    
    [Required] public string Password { get; set; } = default!;

    public bool RememberMe { get; set; } = false;
}