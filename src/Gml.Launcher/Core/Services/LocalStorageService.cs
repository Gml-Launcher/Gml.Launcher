using System;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Gml.Launcher.Core.Exceptions;
using Newtonsoft.Json;
using Splat;
using SQLite;

namespace Gml.Launcher.Core.Services;

public class LocalStorageService : IStorageService
{
    private const string DatabaseFileName = "data.db";
    private readonly SQLiteAsyncConnection _database;

    public LocalStorageService(ISystemService? systemService = null)
    {
        systemService ??= Locator.Current.GetService<ISystemService>()
                          ?? throw new ServiceNotFoundException(typeof(ISystemService));

        var databasePath = System.IO.Path.Combine(systemService.GetGameFolder(true), DatabaseFileName);

        _database = new SQLiteAsyncConnection(databasePath);

        InitializeTables();
    }

    private void InitializeTables()
    {
        _database.CreateTableAsync<StorageItem>().Wait();
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
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
            return JsonConvert.DeserializeObject<T>(storageItem.Value);
        }

        return default(T);
    }

    public Task<int> SaveRecord<T>(T record)
    {
        return _database.InsertOrReplaceAsync(record);
    }

    [Table("StorageItems")]
    private class StorageItem
    {
        [PrimaryKey] public string Key { get; set; } = null!;
        public string? TypeName { get; set; }
        public string Value { get; set; } = null!;
    }
}
