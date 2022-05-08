using Microsoft.AspNetCore.Mvc;

namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class ListAllItemsRequest
{
  [FromRoute]
  public int ProjectId { get; set; }
  
  //[FromQuery]
  //public string? SearchString { get; set; }
}
