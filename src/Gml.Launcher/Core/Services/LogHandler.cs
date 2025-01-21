using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using Gml.Launcher.Core.Exceptions;
using Sentry;

public class LogHandler
{
    private readonly Subject<string> _errorDataSubject = new();
    private readonly Queue<string> _recentLogs = new();
    private readonly List<string> _logBuffer = new();
    private static readonly string[] LogLevels = { "[INFO]", "[WARNING]", "[ERROR]", "[DEBUG]" };
    public Queue<string> RecentLogs => _recentLogs;
    public List<string> LogBuffer => _logBuffer;

    public LogHandler()
    {
        _errorDataSubject
            .Where(data => !string.IsNullOrEmpty(data)
                           && !data.Contains("[gml-patch]", StringComparison.OrdinalIgnoreCase)
                           && !data.Contains("/api/v1/integrations/texture/skins/", StringComparison.OrdinalIgnoreCase)
                           && !data.Contains("/api/v1/integrations/texture/capes/", StringComparison.OrdinalIgnoreCase)
                           )
            .Subscribe(ProcessLogLine);

        Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ => FlushLogBuffer());
    }

    private void ProcessLogLine(string logLine)
    {
        if (LogLevels.Any(level => logLine.Contains(level, StringComparison.OrdinalIgnoreCase)))
        {
            FlushLogBuffer();
        }

        _recentLogs.Enqueue(logLine);

        if (_recentLogs.Count > 30)
        {
            _recentLogs.Dequeue();
        }

        _logBuffer.Add(logLine);
    }

    private void FlushLogBuffer()
    {
        if (_logBuffer.Count > 0)
        {
            HandleErrorData(string.Join(Environment.NewLine, _logBuffer));
            _logBuffer.Clear();
        }
    }

    private void HandleErrorData(string data)
    {
        Debug.WriteLine(data);

        try
        {
            if (IsErrorLog(data))
            {
                var exception = data.Contains("java.")
                    ? ExtractJavaException(data)
                    : ExtractException(data);

                ShowError(ResourceKeysDictionary.GameProfileError, data);
                SentrySdk.CaptureException(exception);
            }
            else if (data.Contains("Exception", StringComparison.OrdinalIgnoreCase))
            {
                ShowError(ResourceKeysDictionary.GameProfileError, data);
                SentrySdk.CaptureException(new MinecraftException(data));
            }
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
    }

    private bool IsErrorLog(string data)
    {
        var errorLogRegex = new Regex(@"\blevel=""ERROR""\b", RegexOptions.Singleline);
        var throwableRegex = new Regex(@"<log4j:Throwable><!\[CDATA\[(.*?)\]\]></log4j:Throwable>", RegexOptions.Singleline);

        return errorLogRegex.IsMatch(data) || throwableRegex.IsMatch(data) ||
               (data.Contains("Exception", StringComparison.OrdinalIgnoreCase) &&
                !data.Contains("INFO", StringComparison.OrdinalIgnoreCase) &&
                !data.Contains("WARN", StringComparison.OrdinalIgnoreCase));
    }

    private Exception ExtractException(string data)
    {
        var throwableRegex = new Regex(@"<log4j:Throwable><!\[CDATA\[(.*?)\]\]></log4j:Throwable>", RegexOptions.Singleline);
        var match = throwableRegex.Match(data);

        if (match.Success)
        {
            var stackTrace = match.Groups[1].Value;
            return new MinecraftException(stackTrace);
        }

        return new MinecraftException(data);
    }

    private Exception ExtractJavaException(string data)
    {
        var javaExceptionRegex = new Regex(@"(java\.(.*?)\.)Exception: (.*?)\n", RegexOptions.Singleline);
        var match = javaExceptionRegex.Match(data);

        if (match.Success)
        {
            var exceptionMessage = match.Groups[3].Value;
            return new MinecraftException(exceptionMessage);
        }

        return new MinecraftException(data);
    }

    private void ShowError(string key, string message)
    {
    }

    private static class ResourceKeysDictionary
    {
        public static string GameProfileError = "GameProfileError";
    }

    public void ProcessLogs(string data)
    {
        _errorDataSubject.OnNext(data);
    }
}
