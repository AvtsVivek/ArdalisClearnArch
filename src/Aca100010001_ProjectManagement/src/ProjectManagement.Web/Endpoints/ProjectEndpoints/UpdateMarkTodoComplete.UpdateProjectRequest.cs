using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Web.Endpoints.ProjectEndpoints;

public class UpdateMarkTodoCompleteRequest
{
  public const string Route = "/ProjectUpdateMarkTodoComplete";
  [Required]
  public int ProdjectId { get; set; }
  [Required]
  public int TodoItemId { get; set; }
  //[Required]
  //public string? Name { get; set; }
}
