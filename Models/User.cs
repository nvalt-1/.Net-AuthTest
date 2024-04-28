using Microsoft.AspNetCore.Identity;

namespace AuthTest.Models;

public class User : IdentityUser<string>
{
    /// <summary>
    /// Gets or sets the number of failed login attempts for the current user.
    /// </summary>
    public override int AccessFailedCount { get; set; }
    
    /// <summary>
    /// Gets or sets the email address for this user.
    /// </summary>
    [ProtectedPersonalData] public override string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    /// <value>True if the email address has been confirmed, otherwise false.</value>
    [PersonalData] public override bool EmailConfirmed { get; set; }
    
    /// <summary>
    /// Gets or sets the primary key for this user.
    /// </summary>
    [PersonalData] public override string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    public override bool LockoutEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    /// A value in the past means the user is not locked out.
    /// </remarks>
    public override DateTimeOffset? LockoutEnd { get; set; }
    
    /// <summary>
    /// Gets or sets the normalized email address for this user.
    /// </summary>
    [ProtectedPersonalData] public override string? NormalizedEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the normalized user name for this user.
    /// </summary>
    public override string? NormalizedUserName { get; set; }
    
    /// <summary>
    /// Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    public override string? PasswordHash { get; set; }
    
    /// <summary>
    /// Gets or sets a telephone number for the user.
    /// </summary>
    [ProtectedPersonalData] public override string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets a flag indicating if a user has confirmed their telephone address.
    /// </summary>
    /// <value>True if the telephone number has been confirmed, otherwise false.</value>
    [PersonalData] public override bool PhoneNumberConfirmed { get; set; }
    
    /// <summary>
    /// A random value that must change whenever a users credentials change (password changed, login removed)
    /// </summary>
    public override string? SecurityStamp { get; set; }
    
    /// <summary>
    /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
    /// </summary>
    /// <value>True if 2fa is enabled, otherwise false.</value>
    public override bool TwoFactorEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the user name for this user.
    /// </summary>
    public override string? UserName { get; set; }
}
