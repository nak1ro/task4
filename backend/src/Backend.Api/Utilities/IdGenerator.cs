namespace Backend.Api.Utilities;

public static class IdGenerator
{
    // IMPORTANT: Generates a unique identifier value
    public static string GetUniqIdValue() => Guid.NewGuid().ToString("N");
}
