namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class UpdateMarkTodoCompleteResponse
{
  public UpdateMarkTodoCompleteResponse(ProjectRecord project)
  {
    Project = project;
  }
  public ProjectRecord Project { get; set; }
}
