using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    public interface ITodosRepository
    {
        Task<IEnumerable<TodoItemReadModel>> GetAllAsync();

        Task<TodoItemReadModel> GetAsync(Guid id);
        
        Task<int> SaveOrUpdateAsync(TodoItemReadModel model);

        Task<int> DeleteAsync(Guid id);
    }
}