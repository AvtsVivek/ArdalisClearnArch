namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class ListAllItemsResponse
{
  public ListAllItemsResponse(int projectId, List<ToDoItemRecord> allItems)
  {
    ProjectId = projectId;
    AllItems = allItems;
  }
  public int ProjectId { get; set; }
  public List<ToDoItemRecord> AllItems { get; set; }
}
