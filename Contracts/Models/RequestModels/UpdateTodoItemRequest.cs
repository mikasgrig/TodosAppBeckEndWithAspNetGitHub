using Contracts.Enums;

namespace Contracts.Models.RequestModels
{
    public class UpdateTodoItemRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }
        
        public Difficulty Difficulty { get; set; }
    }
}