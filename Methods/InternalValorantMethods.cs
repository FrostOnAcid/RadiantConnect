﻿using RadiantConnect.Network.LocalEndpoints;
using System.Diagnostics;
using System.Text.Json;
namespace RadiantConnect.Methods;
// ReSharper disable All

public class InternalValorantMethods
{
    public static bool IsValorantProcessRunning() { return Process.GetProcessesByName("VALORANT").Length > 0; }

    internal static async Task<bool> IsReady(LocalEndpoints localEndpoints)
    {
        try
        {
            JsonElement? response = await localEndpoints.GetHelpAsync();
            bool exists = response?.TryGetProperty("events", out _) ?? false;
            return exists;
        }
        catch { return false; }
    }
}
