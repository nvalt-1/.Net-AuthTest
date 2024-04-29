namespace AuthTest.Services;

public interface IDatabaseService : IDisposable
{
    /// <summary>
    /// Executes a stored procedure, with optional parameters, without a return value.
    ///
    /// Returns false on error.
    /// </summary>
    /// <param name="procName">Stored procedure to execute</param>
    /// <param name="parameters">Parameters to pass to stored procedure</param>
    public Task<bool> Execute(string procName, IDictionary<string, object>? parameters);
    
    /// <summary>
    /// Executes a stored procedure, with optional parameters, returning a list
    /// of rows represented by a dictionary corresponding to columns and values.
    /// 
    /// Returns null on error.
    /// </summary>
    /// <param name="procName">Stored procedure to execute</param>
    /// <param name="parameters">Parameters to pass to stored procedure</param>
    public Task<IList<Dictionary<string, string>>?> Query(string procName, IDictionary<string, object>? parameters);
}

public class DummyDatabase : IDatabaseService
{
    private static readonly Dictionary<string, Func<IDictionary<string, object>?, Task<IList<Dictionary<string, string>>?>>> StoredProcedures = new()
    {
        { "findById",  FindById },
        { "findByUsername", FindByUsername },
        { "insertUser", InsertUser },
        { "deleteUser", DeleteUser },
        { "updateUsername", UpdateUsername },
        { "updateUser", UpdateUser },
        { "updatePassword", UpdatePassword },
        { "incrementAccessFailedCount", IncrementAccessFailedCount },
        { "resetAccessFailedCount", ResetAccessFailedCount },
        { "setLockoutEnabled", SetLockoutEnabled },
        { "setLockoutEnd", SetLockoutEnd },
        { "setSecurityStamp", SetSecurityStamp }
    };

    private static readonly List<Dictionary<string, string>> Table = [];
    private static int _idCount = 1;

    public async Task<bool> Execute(string procName, IDictionary<string, object>? parameters)
    {
        if (!StoredProcedures.ContainsKey(procName))
        {
            return false;
        }
        
        var result = await StoredProcedures[procName](parameters);
        return result != null;
    }

    public Task<IList<Dictionary<string, string>>?> Query(string procName, IDictionary<string, object>? parameters)
    {
        if (!StoredProcedures.ContainsKey(procName))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        return StoredProcedures[procName](parameters);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private static Task<IList<Dictionary<string, string>>?> FindById(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var results = Table.Where(row => row["ID"] == userId).ToList();
        return Task.FromResult<IList<Dictionary<string, string>>?>(results);
    }
    
    private static Task<IList<Dictionary<string, string>>?> FindByUsername(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var username = parameters["@username"] as string;
        if (string.IsNullOrEmpty(username))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var results = Table.Where(row => row["USERNAME"].ToUpper() == username).ToList();
        return Task.FromResult<IList<Dictionary<string, string>>?>(results);
    }

    private static Task<IList<Dictionary<string, string>>?> InsertUser(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        var row = new Dictionary<string, string>()
        {
            { "ACCESS_FAILED_COUNT" , parameters["@accessFailedCount"].ToString() ?? "0" },
            { "EMAIL" , parameters["@email"] as string ?? string.Empty },
            { "EMAIL_CONFIRMED" , parameters["@emailConfirmed"] is not bool ? "0" : (bool)parameters["@emailConfirmed"] ? "1" : "0" },
            { "ID" , string.IsNullOrEmpty(parameters["@id"] as string) ? _idCount.ToString() : parameters["@id"] as string ?? "" },
            { "LOCKOUT_ENABLED" , parameters["@lockoutEnabled"] is not bool ? "0" : (bool)parameters["@lockoutEnabled"] ? "1" : "0" },
            { "LOCKOUT_END" , (parameters["@lockoutEnd"] is not DateTimeOffset ? string.Empty : parameters["@lockoutEnd"].ToString()) ?? string.Empty },
            { "CONCURRENCY_STAMP" , parameters["@concurrencyStamp"] as string ?? string.Empty },
            { "USERNAME" , parameters["@username"] as string ?? string.Empty },
            { "PASSWORD_HASH" , parameters["@passwordHash"] as string ?? string.Empty },
            { "PHONE_NUMBER" , parameters["@phoneNumber"] as string ?? string.Empty },
            { "PHONE_CONFIRMED" , parameters["@phoneConfirmed"] is not bool ? "0" : (bool)parameters["@phoneConfirmed"] ? "1" : "0" },
            { "SECURITY_STAMP" , parameters["@securityStamp"] as string ?? string.Empty },
            { "TWO_FACTOR_ENABLED" , parameters["@twoFactorEnabled"] is not bool ? "0" : (bool)parameters["@twoFactorEnabled"] ? "1" : "0" },
        };
        Table.Add(row);
        _idCount++;
        
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }
    
    private static Task<IList<Dictionary<string, string>>?> DeleteUser(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        Table.Remove(row); 
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> UpdateUsername(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        var username = parameters["@username"] as string;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["USERNAME"] = username;
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> UpdateUser(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        row["ACCESS_FAILED_COUNT"] = parameters["@accessFailedCount"].ToString() ?? "0";
        row["EMAIL"] = parameters["@email"] as string ?? string.Empty;
        row["EMAIL_CONFIRMED"] = parameters["@emailConfirmed"] is not bool ? "0" : (bool)parameters["@emailConfirmed"] ? "1" : "0";
        row["ID"] = parameters["@id"] as string ?? string.Empty;
        row["LOCKOUT_ENABLED"] = parameters["@lockoutEnabled"] is not bool ? "0" : (bool)parameters["@lockoutEnabled"] ? "1" : "0";
        row["LOCKOUT_END"] = (parameters["@lockoutEnd"] is not DateTimeOffset ? string.Empty : parameters["@lockoutEnd"].ToString()) ?? string.Empty;
        row["CONCURRENCY_STAMP"] = parameters["@concurrencyStamp"] as string ?? string.Empty;
        row["USERNAME"] = parameters["@username"] as string ?? string.Empty;
        row["PASSWORD_HASH"] = parameters["@passwordHash"] as string ?? string.Empty;
        row["PHONE_NUMBER"] = parameters["@phoneNumber"] as string ?? string.Empty;
        row["PHONE_CONFIRMED"] = parameters["@phoneConfirmed"] is not bool ? "0" : (bool)parameters["@phoneConfirmed"] ? "1" : "0";
        row["SECURITY_STAMP"] = parameters["@securityStamp"] as string ?? string.Empty;
        row["TWO_FACTOR_ENABLED"] = parameters["@twoFactorEnabled"] is not bool ? "0" : (bool)parameters["@twoFactorEnabled"] ? "1" : "0";
        
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>(){row});
    }
    
    private static Task<IList<Dictionary<string, string>>?> UpdatePassword(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var username = parameters["@username"] as string;
        var passwordHash = parameters["@passwordHash"] as string;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordHash))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["USERNAME"] == username);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["PASSWORD_HASH"] = passwordHash;
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> IncrementAccessFailedCount(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        if (string.IsNullOrEmpty(row["ACCESS_FAILED_COUNT"]))
        {
            row["ACCESS_FAILED_COUNT"] = "1";
        }
        else
        {
            row["ACCESS_FAILED_COUNT"] = (int.Parse(row["ACCESS_FAILED_COUNT"]) + 1).ToString();
        }
        
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }
    
    private static Task<IList<Dictionary<string, string>>?> ResetAccessFailedCount(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["ACCESS_FAILED_COUNT"] = "0";
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> SetLockoutEnabled(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        if (string.IsNullOrEmpty(userId) || parameters["@enabled"] is not bool enabled)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["LOCKOUT_ENABLED"] = enabled ? "1" : "0";
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> SetLockoutEnd(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        var lockoutEnd = parameters["@lockoutEnd"] as DateTimeOffset?;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["LOCKOUT_END"] = (lockoutEnd == null ? "" : lockoutEnd.ToString()) ?? string.Empty;
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

    private static Task<IList<Dictionary<string, string>>?> SetSecurityStamp(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        
        var userId = parameters["@id"] as string;
        var stamp = parameters["@stamp"] as string;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        var row = Table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["SECURITY_STAMP"] = stamp ?? string.Empty;
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }
    
}
