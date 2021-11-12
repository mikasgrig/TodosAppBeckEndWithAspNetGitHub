using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    class TodosRepository : ITodosRepository
    {
        private const string TableName = "Todos";
        private readonly ISqlClient _sqlClient;

        public TodosRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<IEnumerable<TodoItemReadModel>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {TableName}";
            
            return _sqlClient.QueryAsync<TodoItemReadModel>(sql);
        }

        public Task<TodoItemReadModel> GetAsync(Guid id)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Id = @Id";
            
            return _sqlClient.QuerySingleOrDefaultAsync<TodoItemReadModel>(sql, new {Id = id});
        }

        public Task<int> SaveOrUpdateAsync(TodoItemReadModel model)
        {
            var sql = @$"INSERT INTO {TableName} (Id, Title, Description, Difficulty, DateCreated, IsDone) 
                        VALUES (@Id, @Title, @Description, @Difficulty, @DateCreated, @IsDone)
                        ON DUPLICATE KEY UPDATE Title = @Title, Description = @Description, Difficulty = @Difficulty, IsDone = @IsDone";

            return _sqlClient.ExecuteAsync(sql, new
            {
                model.Id,
                model.Title,
                model.Description,
                Difficulty = model.Difficulty.ToString(),
                model.DateCreated,
                model.IsDone
            });
        }

        public Task<int> DeleteAsync(Guid id)
        {
            var sql = $"DELETE FROM {TableName} WHERE Id = @Id";
            
            return _sqlClient.ExecuteAsync(sql, new {Id = id});
        }
    }
}