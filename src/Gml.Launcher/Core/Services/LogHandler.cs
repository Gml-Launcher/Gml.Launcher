using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using Sentry;

namespace Gml.Launcher.Core.Services;

public class LogHandler
{
    private readonly Subject<string> _errorDataSubject = new();


    public LogHandler()
    {
        var errorDataObservable = _errorDataSubject
            .Where(data => !string.IsNullOrEmpty(data))
            .Buffer(() => _errorDataSubject.Where(data => data.Contains("</log4j:Event>")))
            .Select(lines => string.Join(Environment.NewLine, lines));

        errorDataObservable.Subscribe(HandleErrorData);
    }

    private void HandleErrorData(string data)
    {
        Debug.WriteLine(data);

        if (data.Contains("Exception", StringComparison.OrdinalIgnoreCase) ||
            data.Contains("<log4j:Throwable>", StringComparison.OrdinalIgnoreCase))
        {
            var exception = ExtractException(data);
            ShowError(ResourceKeysDictionary.GameProfileError, data);
            SentrySdk.CaptureException(exception);
        }

        if (data.Contains("[gml-patch]", StringComparison.OrdinalIgnoreCase))
        {
        }
        else
        {
            ShowError(ResourceKeysDictionary.GameProfileError, data);
            SentrySdk.CaptureException(new Exception(data));
        }
    }

    private Exception ExtractException(string data)
    {
        var throwableRegex = new Regex(@"<log4j:Throwable><!\[CDATA\[(.*?)\]\]></log4j:Throwable>", RegexOptions.Singleline);
        var match = throwableRegex.Match(data);

        if (match.Success)
        {
            var stackTrace = match.Groups[1].Value;
            return new Exception(stackTrace);
        }

        return new Exception(data);
    }

    private void ShowError(string key, string message)
    {
        // Реализуйте здесь показ ошибок пользователю
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
