using System.Diagnostics;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;

namespace GamerVII.Launcher.Services.GameLaunchService;

/// <summary>
/// Represents a service for launching and managing Minecraft game clients.
/// </summary>
public interface IGameLaunchService
{
    /// <summary>
    /// Delegate for handling the event when loading a client ends.
    /// </summary>
    /// <param name="loadClientEnded">True if loading the client ended successfully, otherwise false.</param>
    /// <param name="message">An optional message related to the loading outcome.</param>
    delegate void LoadClientEventHandler(bool loadClientEnded, string? message);

    /// <summary>
    /// Delegate for handling the event when the progress changes during client loading or downloading.
    /// </summary>
    /// <param name="percentage">The percentage of progress completed.</param>
    delegate void ProgressChangedEventHandler(decimal percentage);

    /// <summary>
    /// Delegate for handling the event when a file changes during client loading or downloading.
    /// </summary>
    /// <param name="message">The message related to the file change.</param>
    delegate void FileChangedEventHandler(string message);

    /// <summary>
    /// Event raised when loading a client ends.
    /// </summary>
    event LoadClientEventHandler LoadClientEnded;

    /// <summary>
    /// Event raised when the progress changes during client loading or downloading.
    /// </summary>
    event ProgressChangedEventHandler ProgressChanged;

    /// <summary>
    /// Event raised when a file changes during client loading or downloading.
    /// </summary>
    event FileChangedEventHandler FileChanged;

    /// <summary>
    /// Launches a specific Minecraft client.
    /// </summary>
    /// <param name="client">The Minecraft client instance to launch.</param>
    /// <param name="user">The user for whom the client is being launched.</param>
    /// <returns>The process responsible for launching the client.</returns>
    Task<Process> LaunchClient(IGameClient client, IUser user);

    /// <summary>
    /// Downloads a Minecraft client of a certain version.
    /// </summary>
    /// <param name="client">The Minecraft client instance to download.</param>
    /// <returns>Client information required to run.</returns>
    Task<IGameClient> LoadClient(IGameClient client);
}
