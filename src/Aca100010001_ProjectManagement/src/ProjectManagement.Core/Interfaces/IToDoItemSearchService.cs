using Ardalis.Result;
using ProjectManagement.Core.ProjectAggregate;

namespace ProjectManagement.Core.Interfaces;

public interface IToDoItemSearchService
{
  Task<Result<ToDoItem>> GetNextIncompleteItemAsync(int projectId);
  Task<Result<List<ToDoItem>>> GetAllIncompleteItemsAsync(int projectId, string searchString);
  Task<Result<List<ToDoItem>>> GetAllItemsAsync(int projectId);
  // AllItems
}
