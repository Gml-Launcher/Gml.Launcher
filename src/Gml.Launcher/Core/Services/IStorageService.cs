using System.Threading;
using System.Threading.Tasks;

namespace Gml.Launcher.Core.Services;

/// <summary>
///     Represents a storage service interface used for saving and retrieving data.
/// </summary>
public interface IStorageService
{
    /// <summary>
    ///     Stores the provided value with the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key for the value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="token"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CancellationToken? token = default);

    /// <summary>
    ///     Retrieves the value associated with the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the retrieved value,
    ///     or <c>null</c> if the key does not exist in the storage.
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    ///     Saves a record of type T to the storage service.
    /// </summary>
    /// <typeparam name="T">The type of the record to save.</typeparam>
    /// <param name="value">The record to save.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveRecord<T>(T value);

    /// <summary>
    ///     Retrieves the logs from the storage service asynchronously.
    /// </summary>
    /// <param name="rowCount">The maximum number of logs to retrieve. Default value is 100.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the logs as a concatenated string.</returns>
    Task<string> GetLogsAsync(int rowCount);
}
