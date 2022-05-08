using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Web.Endpoints.ItemsEndpoints;

public class CreateItemRequest
{
  public const string Route = "/Items";

  [Required]
  public string? Title { get; set; }

  public string? Desctiption { get; set; }

  public int ProjectId { get; set; }
}
