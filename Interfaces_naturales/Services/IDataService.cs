using Interfaces_naturales.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces_naturales.Services
{
    public interface IDataService
    {
        Task<List<SpeechHistoryItem>> GetHistoryAsync();
        Task SaveHistoryItemAsync(SpeechHistoryItem item);
        Task ClearHistoryAsync();
        Task DeleteHistoryItemAsync(Guid id);
    }
}
