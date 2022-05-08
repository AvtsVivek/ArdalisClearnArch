using Ardalis.ApiEndpoints;
using ProjectManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class ListAllItems : EndpointBaseAsync
    .WithRequest<ListAllItemsRequest>
    .WithActionResult<ListAllItemsResponse>
{
  private readonly IToDoItemSearchService _searchService;

  public ListAllItems(IToDoItemSearchService searchService)
  {
    _searchService = searchService;
  }

  [HttpGet("/Projects/{ProjectId}/AllItems")]
  [SwaggerOperation(
      Summary = "Gets a list of a project's all items",
      Description = "Gets a list of a project's all items",
      OperationId = "Project.ListAllItems",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<ListAllItemsResponse>> HandleAsync([FromQuery] ListAllItemsRequest request, CancellationToken cancellationToken)
  {
    var response = new ListAllItemsResponse(0, new List<ToDoItemRecord>());
    
    var result = await _searchService.GetAllItemsAsync(request.ProjectId);

    if (result.Status == Ardalis.Result.ResultStatus.Ok)
    {
      response.ProjectId = request.ProjectId;
      response.AllItems = new List<ToDoItemRecord>(
              result.Value.Select(
                  item => new ToDoItemRecord(item.Id,
                  item.Title,
                  item.Description,
                  item.IsDone)));
    }
    else if (result.Status == Ardalis.Result.ResultStatus.Invalid)
    {
      return BadRequest(result.ValidationErrors);
    }
    else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
    {
      return NotFound();
    }

    return Ok(response);
  }
}
