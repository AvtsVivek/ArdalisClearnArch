using Ardalis.ApiEndpoints;
using ProjectManagement.Core.ProjectAggregate;
using ProjectManagement.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ProjectManagement.Core.ProjectAggregate.Specifications;

namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class UpdateMarkTodoComplete : EndpointBaseAsync
    .WithRequest<UpdateMarkTodoCompleteRequest>
    .WithActionResult<UpdateProjectResponse>
{
  private readonly IRepository<Project> _repository;

  public UpdateMarkTodoComplete(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpPut(UpdateMarkTodoCompleteRequest.Route)]
  [SwaggerOperation(
      Summary = "Updates a Project; marks a todo item to complete",
      Description = "Marks a given todo item to complete",
      OperationId = "Projects.Update.Todo",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<UpdateProjectResponse>> HandleAsync(UpdateMarkTodoCompleteRequest request,
      CancellationToken cancellationToken)
  {
    //if (request.Name == null)
    //{
    //  return BadRequest();
    //}
    //var existingProject = await _repository.GetByIdAsync(request.Id); // TODO: pass cancellation token

    //if (existingProject == null)
    //{
    //  return NotFound();
    //}

    // existingProject.UpdateName(request.Name);

    var projectSpec = new ProjectByIdWithItemsSpec(request.ProdjectId);
    var project = await _repository.GetBySpecAsync(projectSpec);

    if (project == null)
    {
      return NotFound();
    }

    var itemToBeUpdated = project.Items.FirstOrDefault(item => item.Id == request.TodoItemId);

    if (itemToBeUpdated == null)
    {
      return NotFound($"Todo item with id {request.TodoItemId} is not found");
    }

    itemToBeUpdated.MarkComplete();

    await _repository.UpdateAsync(project); // TODO: pass cancellation token

    var response = new UpdateMarkTodoCompleteResponse(
      project: new ProjectRecord(project.Id, project.Name)
    );
    return Ok(response);
  }
}
