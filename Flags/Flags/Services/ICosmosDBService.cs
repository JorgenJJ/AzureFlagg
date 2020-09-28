using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flags.Models;

namespace Flags.Services
{
    public interface ICosmosDBService
    {
        Task<IEnumerable<Quiz>> GetItemsAsync(string query);
        Task<Quiz> GetItemAsync(string id);
        Task AddItemAsync(Quiz item);
        Task UpdateItemAsync(string id, Quiz item);
        Task DeleteItemAsync(string id);
    }
}
