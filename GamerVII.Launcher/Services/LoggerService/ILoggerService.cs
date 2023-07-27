using System;

namespace GamerVII.Launcher.Services.LoggerService;

/// <summary>
/// Defines a logger service contract for logging messages.
/// </summary>
public interface ILoggerService : IDisposable
{
    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    void Log(string message);
}