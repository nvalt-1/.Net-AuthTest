using AuthTest.Models;
using AuthTest.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthTest.Classes;

public class UserStore<TUser> : IUserPasswordStore<TUser> where TUser : User
{
    private readonly IDatabaseService _databaseService;
    
    public UserStore(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    #region IUserStore Implementation
    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        var parameters = CreateParametersFromUser(user);
        var result = await _databaseService.Execute("insertUser", parameters);
        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, object> { { "@id", user.Id } };
        var result = await _databaseService.Execute("deleteUser", parameters);
        return result ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, object> { { "@id", userId } };
        var rows = await _databaseService.Query("findById", parameters);
        
        if (rows == null || rows.Count != 1)
        {
            return null;
        }

        return CreateUserFromRow(rows[0]);
    }

    public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, object> { { "@username", normalizedUserName } };
        var rows = await _databaseService.Query("findByUsername", parameters);
        
        if (rows == null || rows.Count != 1)
        {
            return null;
        }

        return CreateUserFromRow(rows[0]);
    }

    public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }
    
    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public async Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, object> { { "@id", user.Id }, { "@username", user.UserName ?? "" } };
        var result = await _databaseService.Execute("updateUsername", parameters);
        if (result)
        {
            user.UserName = userName;
        } 
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        var parameters = CreateParametersFromUser(user);
        var result = await _databaseService.Execute("updateUser", parameters);
        return result ? IdentityResult.Success : IdentityResult.Failed();
    }
    #endregion

    #region IUserPasswordStore Implementation

    public Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
    }

    public async Task SetPasswordHashAsync(TUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        var parameters = new Dictionary<string, object> { { "@username", user.UserName ?? "" }, { "@passwordHash", passwordHash ?? "" } };
        await _databaseService.Execute("updatePassword", parameters);
    }
    #endregion
    
    public void Dispose()
    {
        _databaseService.Dispose();
        GC.SuppressFinalize(this);
    }

    private static TUser CreateUserFromRow(IDictionary<string, string> row)
    {
        var user = Activator.CreateInstance<TUser>();
        
        user.AccessFailedCount = string.IsNullOrEmpty(row["ACCESS_FAILED_COUNT"]) ? 0 : int.Parse(row["ACCESS_FAILED_COUNT"]);
        user.Email = string.IsNullOrEmpty(row["EMAIL"]) ? null : row["EMAIL"];
        user.EmailConfirmed = row["EMAIL_CONFIRMED"] == "1";
        user.Id = row["ID"];
        user.LockoutEnabled = row["LOCKOUT_ENABLED"] == "1";
        user.LockoutEnd = string.IsNullOrEmpty(row["LOCKOUT_END"]) ? null : DateTimeOffset.Parse(row["LOCKOUT_END"]);
        user.NormalizedEmail = string.IsNullOrEmpty(row["EMAIL"]) ? null : row["EMAIL"].ToUpper();
        user.NormalizedUserName = string.IsNullOrEmpty(row["USERNAME"]) ? null : row["USERNAME"].ToUpper();
        user.PasswordHash = string.IsNullOrEmpty(row["PASSWORD_HASH"]) ? null : row["PASSWORD_HASH"];
        user.PhoneNumber = string.IsNullOrEmpty(row["PHONE_NUMBER"]) ? null : row["PHONE_NUMBER"];
        user.PhoneNumberConfirmed = row["PHONE_CONFIRMED"] == "1";
        user.SecurityStamp = string.IsNullOrEmpty(row["SECURITY_STAMP"]) ? null : row["SECURITY_STAMP"];
        user.TwoFactorEnabled = row["TWO_FACTOR_ENABLED"] == "1";
        user.UserName = string.IsNullOrEmpty(row["USERNAME"]) ? null : row["USERNAME"];
        
        return user;
    }

    private static Dictionary<string, object> CreateParametersFromUser(TUser user)
    {
        return new Dictionary<string, object>
        {
            { "@accessFailedCount" , user.AccessFailedCount },
            { "@email" , user.Email ?? "" },
            { "@emailConfirmed" , user.EmailConfirmed },
            { "@id" , user.Id },
            { "@lockoutEnabled" , user.LockoutEnabled },
            { "@lockoutEnd" , user.LockoutEnd == null ? "" : user.LockoutEnd},
            { "@concurrencyStamp" , user.ConcurrencyStamp ?? Guid.NewGuid().ToString() },
            { "@username" , user.UserName ?? "" },
            { "@passwordHash" , user.PasswordHash ?? "" },
            { "@phoneNumber" , user.PhoneNumber ?? "" },
            { "@phoneConfirmed" , user.PhoneNumberConfirmed },
            { "@securityStamp" , user.SecurityStamp ?? Guid.NewGuid().ToString() },
            { "@twoFactorEnabled" , user.TwoFactorEnabled },
        };
    }
}