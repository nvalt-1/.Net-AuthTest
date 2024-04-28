using System.ComponentModel.DataAnnotations;

namespace AuthTest.Models.Requests;

public class RegisterRequest
{
    [Required] public string Username { get; set; } = default!;

    [Required] public string Password { get; set; } = default!;
}