using Interfaces_naturales.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Interfaces_naturales.Services
{
    public class JsonDataService : IDataService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "speech_history.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<SpeechHistoryItem>> GetHistoryAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<SpeechHistoryItem>();

                var json = await File.ReadAllTextAsync(_filePath);
                var history = JsonSerializer.Deserialize<List<SpeechHistoryItem>>(json, _jsonOptions);
                return history ?? new List<SpeechHistoryItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading history: {ex.Message}");
                return new List<SpeechHistoryItem>();
            }
        }

        public async Task SaveHistoryItemAsync(SpeechHistoryItem item)
        {
            try
            {
                var history = await GetHistoryAsync();
                history.Insert(0, item);

                // Mantener solo los últimos X elementos, cambiar luego a variable en conf por ahora 50
                if (history.Count > 50)
                    history = history.Take(50).ToList();

                var json = JsonSerializer.Serialize(history, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history: {ex.Message}");
            }
        }

        public async Task ClearHistoryAsync()
        {
            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing history: {ex.Message}");
            }
        }

        public async Task DeleteHistoryItemAsync(Guid id)
        {
            try
            {
                var history = await GetHistoryAsync();
                history.RemoveAll(h => h.Id == id);
                var json = JsonSerializer.Serialize(history, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting history item: {ex.Message}");
            }
        }
    }
}
