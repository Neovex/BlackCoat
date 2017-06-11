using System;
using System.Linq;
using System.Diagnostics;

// TODO : move to tools DLL

/// <summary>
/// Log Severity
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}

/// <summary>
/// Generic Logger
/// </summary>
public static class Log
{
    public static event Action<String> OnLog = Console.WriteLine;

    public static LogLevel Level = LogLevel.Debug;

    public static void Debug(params Object[] msgs)
    {
        DoLog(msgs, LogLevel.Debug);
    }

    public static void Info(params Object[] msgs)
    {
        DoLog(msgs, LogLevel.Info);
    }

    public static void Warning(params Object[] msgs)
    {
        DoLog(msgs, LogLevel.Warning);
    }

    public static void Error(params Object[] msgs)
    {
        DoLog(msgs, LogLevel.Error);
    }

    public static void Fatal(params Object[] msgs)
    {
        DoLog(msgs, LogLevel.Fatal);
    }

    private static void DoLog(Object[] msgs, LogLevel lvl)
    {
        if (lvl < Level) return;

        var callingMethod = new StackFrame(0).GetMethod();
        var skip = 1;
        while (skip < 8 &&
              callingMethod == null ||
              callingMethod.DeclaringType == null ||
              callingMethod.DeclaringType == typeof(Log))
        {
            // Dig deeper into the stack
            callingMethod = new StackFrame(skip++).GetMethod();
        }
        var msg = String.Concat(DateTime.Now.ToLongTimeString(), " - ", lvl, " - ", callingMethod.DeclaringType.Name, ".", callingMethod.Name, "(): ",
                                String.Join(" ", msgs.Select(m => (m ?? "[NULL]").ToString())));
        OnLog(msg);
    }
}