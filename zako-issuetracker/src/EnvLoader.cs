using System;

namespace zako_issuetracker;

public static class EnvLoader
{
    private static Dictionary<string, string?> GetEnv()
    {
        Dictionary<string, string?> EnvDict = new Dictionary<string, string?>();
        EnvDict.Add("DISCORD_TOKEN", Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? null);
        EnvDict.Add("SQLITE_FILE", Environment.GetEnvironmentVariable("SQLITE_FILE") ?? null);

        return EnvDict;
    }

    public static string? GetToken()
    {
        return GetEnv()["DISCORD_TOKEN"] ?? null;
    }

    public static string? GetSqlitePath()
    {
        return GetEnv()["SQLITE_FILE"] ?? null;
    }
}
