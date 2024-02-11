using Godot;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public static class Debug
{
    public const bool PRINT_ENABLED = true;

    public static int Indent = 0;

    public static List<DebugAction> RegisteredActions = new();

    public static event System.Action<DebugAction> OnActionAdded;

    private static string IndentString => GetIndentString();

    public static void Log(object o)
    {
        var message = o == null ? "null" : o.ToString();
        Log(message);
    }

    public static void Log(bool debug, string message)
    {
        if (debug)
        {
            Log(message);
        }
    }

    public static void Log(string message)
    {
        if (PRINT_ENABLED)
        {
            string s = IndentString + message;
            GD.Print(s);
        }
    }

    public static void LogMethod(object obj = null, [CallerFilePath] string file = "", [CallerMemberName] string caller = "")
    {
        if (string.IsNullOrEmpty(caller)) return;
        if (string.IsNullOrEmpty(file)) return;
        var filename = Path.GetFileNameWithoutExtension(file);
        var message = obj?.ToString() ?? "";
        Log(string.IsNullOrEmpty(message) ? $"{filename}.{caller}" : $"{filename}.{caller}: {message}");
    }

    public static void LogError(string message)
    {
        if (PRINT_ENABLED)
        {
            GD.PrintErr(IndentString + message);
        }
    }

    public static void AddIndent() => Indent++;

    public static void RemoveIndent() => Indent--;

    private static string GetIndentString()
    {
        string s = "";

        for (int i = 0; i < Indent; i++)
        {
            s += "  ";
        }

        return s;
    }

    public static void RegisterAction(DebugAction action)
    {
        RegisteredActions.Add(action);
        OnActionAdded?.Invoke(action);
    }
}

public class DebugAction
{
    public string Text { get; set; }

    public string Category { get; set; }

    public System.Action<DebugView> Action { get; set; }
}
