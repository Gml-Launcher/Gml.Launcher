using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Gml.Client;
using Gml.Launcher.Core.Exceptions;
using Splat;
using SQLite;

namespace Gml.Launcher.Core.Services;

public class LocalStorageService : IStorageService
{
    private const string DatabaseFileName = "data.db";
    private readonly SQLiteAsyncConnection _database;
    private readonly IGmlClientManager _gmlClient;
    private readonly ISystemService _systemService;

    public LocalStorageService(ISystemService? systemService = null, IGmlClientManager? gmlClient = null)
    {
        _gmlClient = gmlClient
                     ?? Locator.Current.GetService<IGmlClientManager>()
                     ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _systemService ??= Locator.Current.GetService<ISystemService>()
                          ?? throw new ServiceNotFoundException(typeof(ISystemService));

        var databasePath = System.IO.Path.Combine(_systemService.GetGameFolder(_gmlClient.ProjectName ,true), DatabaseFileName);

        _database = new SQLiteAsyncConnection(databasePath);

        InitializeTables();
    }

    private void InitializeTables()
    {
        _database.CreateTableAsync<StorageItem>().Wait();
        _database.CreateTableAsync<LogsItem>().Wait();
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken? token = default)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        var storageItem = new StorageItem
        {
            Key = key,
            TypeName = typeof(T).FullName,
            Value = serializedValue
        };

        await _database.InsertOrReplaceAsync(storageItem);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var storageItem = await _database.Table<StorageItem>()
            .Where(si => si.Key == key)
            .FirstOrDefaultAsync();

        if (storageItem != null)
        {
            return JsonSerializer.Deserialize<T>(storageItem.Value);
        }

        return default;
    }

    public Task<int> SaveRecord<T>(T record)
    {
        return _database.InsertOrReplaceAsync(record);
    }

    public async Task<string> GetLogsAsync(int rowCount = 100)
    {
        var logs = await _database.Table<LogsItem>().Take(rowCount).ToListAsync();

        return string.Join("\n", logs.Select(c => c.Message));
    }

    [Table("StorageItems")]
    private class StorageItem
    {
        [PrimaryKey] public string Key { get; set; } = null!;
        public string? TypeName { get; set; }
        public string Value { get; set; } = null!;
    }

    [Table("Logs")]
    private class LogsItem
    {
        [PrimaryKey] public string Date { get; set; } = null!;
        public string? Message { get; set; }
        public string StackTrace { get; set; } = null!;
    }
}
