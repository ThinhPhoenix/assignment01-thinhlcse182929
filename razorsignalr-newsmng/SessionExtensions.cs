//Make this file at same level of Program.cs (Razor)
using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class SessionExtensions
{
    /// <summary>
    /// Stores an object in the session as a JSON string.
    /// </summary>
    public static void SetObject<T>(this ISession session, string key, T value)
    {
        if (value == null) return;
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    /// <summary>
    /// Retrieves an object from the session by deserializing the JSON string.
    /// Returns default(T) if the key is not found.
    /// </summary>
    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
