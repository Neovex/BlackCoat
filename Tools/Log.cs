using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Log Severity
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

/// <summary>
/// Generic Logger
/// </summary>
public static class Log
{
    /// <summary>
    /// Occurs when a message is logged.
    /// </summary>
    public static event Action<String, LogLevel> OnLog = (m, l) => Console.WriteLine(m);

    /// <summary>
    /// The current log Severity
    /// </summary>
    public static LogLevel Level = LogLevel.Debug;

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="msgs">The objects and or messages to log</param>
    public static void Debug(params Object[] msgs) => DoLog(msgs, LogLevel.Debug);
    /// <summary>
    /// Logs a info message
    /// </summary>
    /// <param name="msgs">The objects and or messages to log</param>
    public static void Info(params Object[] msgs) => DoLog(msgs, LogLevel.Info);
    /// <summary>
    /// Logs a warning
    /// </summary>
    /// <param name="msgs">The objects and or messages to log</param>
    public static void Warning(params Object[] msgs) => DoLog(msgs, LogLevel.Warn);
    /// <summary>
    /// Logs an error
    /// </summary>
    /// <param name="msgs">The objects and or messages to log</param>
    public static void Error(params Object[] msgs) => DoLog(msgs, LogLevel.Error);
    /// <summary>
    /// Logs a fatal error
    /// </summary>
    /// <param name="msgs">The objects and or messages to log</param>
    public static void Fatal(params Object[] msgs) => DoLog(msgs, LogLevel.Fatal);

    private static void DoLog(Object[] msgs, LogLevel lvl)
    {
        if (lvl < Level) return;

        int skip = 2;// Skip Log layer
        MethodBase callingMethod = null;
        bool anonymousMethodOnTop = false;
        while (callingMethod == null || callingMethod.DeclaringType == null || IsAnonymous(callingMethod))
        {
            // Dig into the stack
            callingMethod = new StackFrame(skip++).GetMethod();
            anonymousMethodOnTop = IsAnonymous(callingMethod) || anonymousMethodOnTop;
        }

        var typename = GetFriendlyTypeName(callingMethod.DeclaringType);
        var methodName = GetFriendlyMethodName(callingMethod, anonymousMethodOnTop);
        var msgContent = String.Join(" ", msgs.Select(m => (m ?? "[NULL]").ToString()));
        OnLog($"{DateTime.Now.ToLongTimeString()} - {lvl,-5} - {typename}.{methodName}(): {msgContent}", lvl);
    }

    private static bool IsAnonymous(MethodBase method) => method.Name[0] == '<';

    private static string GetFriendlyTypeName(Type type)
    {
        if (!type.IsGenericType) return type.Name;
        return $"{type.Name.Replace('`', '<')}>";
    }

    private static string GetFriendlyMethodName(MethodBase callingMethod, bool anonymous)
    {
        if (anonymous) return $"{GetFriendlyMethodName(callingMethod, false)}=>[Anonymous]";
        return callingMethod.MemberType == MemberTypes.Constructor ? "[Constructor]" : callingMethod.Name;
    }
}