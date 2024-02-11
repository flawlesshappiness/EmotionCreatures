using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

public static class Debug
{
    public const bool PRINT_ENABLED = true;

    public static int Indent = 0;

    public static List<DebugAction> RegisteredActions = new();

    public static event Action<DebugAction> OnActionAdded;

    private static List<LogMessage> _logs = new();

    public static void Log(object o)
    {
        var message = o == null ? "null" : o.ToString();
        Log(message);
    }

    public static void Log(string message)
    {
        var log = new LogMessage
        {
            Message = message,
            Type = LogType.LOG,
            Indent = Indent,
        };

        _logs.Add(log);

        if (PRINT_ENABLED)
        {
            GD.Print(log.GetDebugMessage());
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
        var log = new LogMessage
        {
            Message = message,
            Type = LogType.ERROR,
            Indent = Indent,
        };

        _logs.Add(log);

        if (PRINT_ENABLED)
        {
            GD.PrintErr(log.GetDebugMessage());
        }
    }

    public static void Trace(string message)
    {
        var log = new LogMessage
        {
            Message = message,
            Type = LogType.TRACE,
            Indent = Indent,
        };

        _logs.Add(log);

        if (PRINT_ENABLED)
        {
            GD.Print(log.GetDebugMessage());
        }
    }

    public static void TraceMethod(object obj = null, [CallerFilePath] string file = "", [CallerMemberName] string caller = "")
    {
        if (string.IsNullOrEmpty(caller)) return;
        if (string.IsNullOrEmpty(file)) return;
        var filename = Path.GetFileNameWithoutExtension(file);
        var message = obj?.ToString() ?? "";
        Trace(string.IsNullOrEmpty(message) ? $"{filename}.{caller}" : $"{filename}.{caller}: {message}");
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

    public static void WriteLogsToPersistentData()
    {
        LogMethod();

        var path = $"user://log.txt";
        var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Write);

        foreach (var log in _logs)
        {
            file.StoreLine(log.GetLogMessage());
        }
    }
}

public class DebugAction
{
    public string Text { get; set; }

    public string Category { get; set; }

    public System.Action<DebugView> Action { get; set; }
}

public enum LogType { LOG, TRACE, EXCEPTION, ERROR }

public class LogMessage
{
    public string Message { get; set; }
    public LogType Type { get; set; } = LogType.LOG;
    public int Indent { get; set; } = 0;
    public string UtcTime { get; private set; }
    public string GameTime { get; private set; }

    public LogMessage()
    {
        UtcTime = DateTime.UtcNow.ToString("hh:mm:ss:f", CultureInfo.InvariantCulture);
        GameTime = Time.GetTicksMsec().ToString();
    }

    public string GetDebugMessage()
    {
        return GetIndentString() + Message;
    }

    public string GetLogMessage()
    {
        return $"[{UtcTime}] {Type}: {Message}";
    }

    private string GetIndentString()
    {
        string s = "";

        for (int i = 0; i < Indent; i++)
        {
            s += "  ";
        }

        return s;
    }
}