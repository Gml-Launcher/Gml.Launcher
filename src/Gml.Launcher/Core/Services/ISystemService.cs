using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;

namespace Gml.Launcher.Core.Services;

/// <summary>
///     Represents a system service that provides various system-related information and functionalities.
/// </summary>
public interface ISystemService
{
    /// <summary>
    ///     Retrieves the application folder path based on the current operating system.
    /// </summary>
    /// <returns>The path to the application folder.</returns>
    string GetApplicationFolder();

    /// Retrieves the game folder path.
    /// @param additionalPath The additional path within the game folder. Can be null or empty.
    /// @param needCreate Specify whether the folder should be created if it does not exist.
    /// @return The full path to the game folder, including the additional path if provided.
    /// /
    string GetGameFolder(string additionalPath, bool needCreate);

    /// <summary>
    ///     Gets the maximum amount of RAM available in the system in megabytes.
    /// </summary>
    /// <returns>The maximum amount of RAM available in the system in megabytes.</returns>
    ulong GetMaxRam();

    /// <summary>
    ///     Gets the operating system type.
    /// </summary>
    /// <returns>The operating system type. Possible values are: Undefined, Linux, OsX, Windows.</returns>
    OsType GetOsType();

    /// <summary>
    ///     Gets the available languages.
    /// </summary>
    /// <returns>An enumerable of Language objects representing the available languages.</returns>
    IEnumerable<Language> GetAvailableLanguages();

    /// <summary>
    ///     Gets the hardware identification (HWID) of the system.
    /// </summary>
    /// <returns>The HWID of the system.</returns>
    string GetHwid();

    /// <summary>
    ///     Asynchronously loads system data such as drive list, motherboard list, and CPU list.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LoadSystemData();

    bool IsDiskFull(IOException ioException);

    /// <summary>
    /// Retrieves the architecture of the operating system.
    /// </summary>
    /// <returns>A string representation of the operating system's architecture in lowercase, excluding the 'x' prefix.</returns>
    string GetOsArchitecture();
}
