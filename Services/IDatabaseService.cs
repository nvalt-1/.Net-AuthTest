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
    private static Dictionary<string, Func<IDictionary<string, object>?, Task<IList<Dictionary<string, string>>?>>> _storedProcedures = new()
    {
        { "findById",  FindById},
        { "findByUsername", FindByUsername },
        { "insertUser", InsertUser },
        { "deleteUser", DeleteUser },
        { "updateUsername", UpdateUsername },
        { "updateUser", UpdateUser },
        { "updatePassword", UpdatePassword }
    };

    private static List<Dictionary<string, string>> _table = [];
    private static int idCount = 1;

    public async Task<bool> Execute(string procName, IDictionary<string, object>? parameters)
    {
        if (!_storedProcedures.ContainsKey(procName))
        {
            return false;
        }
        
        var result = await _storedProcedures[procName](parameters);
        return result != null;
    }

    public Task<IList<Dictionary<string, string>>?> Query(string procName, IDictionary<string, object>? parameters)
    {
        if (!_storedProcedures.ContainsKey(procName))
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        return _storedProcedures[procName](parameters);
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

        var results = _table.Where(row => row["ID"] == userId).ToList();
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

        var results = _table.Where(row => row["USERNAME"].ToUpper() == username).ToList();
        return Task.FromResult<IList<Dictionary<string, string>>?>(results);
    }

    private static Task<IList<Dictionary<string, string>>?> InsertUser(IDictionary<string, object>? parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        var row = new Dictionary<string, string>()
        {
            { "ACCESS_FAILED_COUNT" , parameters["@accessFailedCount"] as string ?? string.Empty },
            { "EMAIL" , parameters["@email"] as string ?? string.Empty },
            { "EMAIL_CONFIRMED" , parameters["@emailConfirmed"] as string ?? string.Empty },
            { "ID" , string.IsNullOrEmpty(parameters["@id"] as string) ? idCount.ToString() : parameters["@id"] as string ?? "" },
            { "LOCKOUT_ENABLED" , parameters["@lockoutEnabled"] as string ?? string.Empty },
            { "LOCKOUT_END" , parameters["@lockoutEnd"] as string ?? string.Empty },
            { "CONCURRENCY_STAMP" , parameters["@concurrencyStamp"] as string ?? string.Empty },
            { "USERNAME" , parameters["@username"] as string ?? string.Empty },
            { "PASSWORD_HASH" , parameters["@passwordHash"] as string ?? string.Empty },
            { "PHONE_NUMBER" , parameters["@phoneNumber"] as string ?? string.Empty },
            { "PHONE_CONFIRMED" , parameters["@phoneConfirmed"] as string ?? string.Empty },
            { "SECURITY_STAMP" , parameters["@securityStamp"] as string ?? string.Empty },
            { "TWO_FACTOR_ENABLED" , parameters["@twoFactorEnabled"] as string ?? string.Empty },
        };
        _table.Add(row);
        idCount++;
        
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

        var row = _table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        _table.Remove(row); 
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

        var row = _table.FirstOrDefault(row => row["ID"] == userId);
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
        
        var row = _table.FirstOrDefault(row => row["ID"] == userId);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }
        
        row["ACCESS_FAILED_COUNT"] = parameters["@accessFailedCount"] as string ?? string.Empty;
        row["EMAIL"] = parameters["@email"] as string ?? string.Empty;
        row["EMAIL_CONFIRMED"] = parameters["@emailConfirmed"] as string ?? string.Empty;
        row["ID"] = parameters["@id"] as string ?? string.Empty;
        row["LOCKOUT_ENABLED"] = parameters["@lockoutEnabled"] as string ?? string.Empty;
        row["LOCKOUT_END"] = parameters["@lockoutEnd"] as string ?? string.Empty;
        row["CONCURRENCY_STAMP"] = parameters["@concurrencyStamp"] as string ?? string.Empty;
        row["USERNAME"] = parameters["@username"] as string ?? string.Empty;
        row["PASSWORD_HASH"] = parameters["@passwordHash"] as string ?? string.Empty;
        row["PHONE_NUMBER"] = parameters["@phoneNumber"] as string ?? string.Empty;
        row["PHONE_CONFIRMED"] = parameters["@phoneConfirmed"] as string ?? string.Empty;
        row["SECURITY_STAMP"] = parameters["@securityStamp"] as string ?? string.Empty;
        row["TWO_FACTOR_ENABLED"] = parameters["@twoFactorEnabled"] as string ?? string.Empty;

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

        var row = _table.FirstOrDefault(row => row["USERNAME"] == username);
        if (row == null)
        {
            return Task.FromResult<IList<Dictionary<string, string>>?>(null);
        }

        row["PASSWORD_HASH"] = passwordHash;
        return Task.FromResult<IList<Dictionary<string, string>>?>(new List<Dictionary<string, string>>());
    }

}
