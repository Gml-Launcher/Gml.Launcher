using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.LocalStorage;


public class LocalStorageService : ILocalStorageService
{
    private readonly string _storagePath = "config.data";

    private Dictionary<string, string>? _storage;

    public async Task<T?> GetAsync<T>(string key)
    {
        if (_storage == null)
            _storage = await ReadDataFromFile();


        if (_storage.TryGetValue(key, out string? jsonValue))
        {
            return JsonConvert.DeserializeObject<T>(jsonValue);
        }

        return default;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        if (_storage == null)
            _storage = await ReadDataFromFile();

        _storage[key] = JsonConvert.SerializeObject(value);

        await SaveDataToFile(_storage);
    }

    private async Task SaveDataToFile(Dictionary<string, string> storage)
    {
        using (var fileStream = new FileStream(_storagePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        {
            using (var streamWriter = new StreamWriter(fileStream))
            {
                var json = JsonConvert.SerializeObject(storage);
                await streamWriter.WriteAsync(json);
            }
        }
    }

    private async Task<Dictionary<string, string>> ReadDataFromFile()
    {
        if (File.Exists(_storagePath))
        {

            using (var fileStream = new FileStream(_storagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    string json = await streamReader.ReadToEndAsync();

                    // json = json.Replace("\\\"", string.Empty);
                    Dictionary<string, string> data = new Dictionary<string, string>();

                    try
                    {
                        data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    }
                    catch
                    {
                        
                    }

                    return  data;
                }
            }


        }

        return new Dictionary<string, string>();
    }
}
