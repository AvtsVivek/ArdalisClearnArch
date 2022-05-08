using Ardalis.ApiEndpoints;
using ProjectManagement.Core.ProjectAggregate;
using ProjectManagement.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.Web.Endpoints.ItemsEndpoints;

public class Create : EndpointBaseAsync
    .WithRequest<CreateItemRequest>
    .WithActionResult<CreateItemResponse>
{
  private readonly IRepository<Project> _repository;

  public Create(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpPost("/Projects/AddItem")]
  [SwaggerOperation(
      Summary = "Creates a new Project",
      Description = "Creates a new Project",
      OperationId = "Project.Create",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<CreateItemResponse>> HandleAsync(
    CreateItemRequest request, CancellationToken cancellationToken)
  {
    if (request.Title == null)
    {
      return BadRequest();
    }

    var newTodoItem = new ToDoItem() { Title = request.Title, Description = request.Desctiption ?? String.Empty};

    var project = await _repository.GetByIdAsync(request.ProjectId);

    if(project == null)
      return BadRequest();

    project.AddItem(newTodoItem);

    await _repository.UpdateAsync(project);

    return Ok();
  }
}
