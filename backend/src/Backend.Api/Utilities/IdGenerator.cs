namespace Backend.Api.Utilities;

public static class IdGenerator
{
    public static string GetUniqIdValue() => Guid.NewGuid().ToString("N");
}
