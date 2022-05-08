namespace ProjectManagement.Web.Endpoints.ItemsEndpoints;

public class CreateItemResponse
{
  public CreateItemResponse(int id, string name)
  {
    Id = id;
    Name = name;
  }
  public int Id { get; set; }
  public string Name { get; set; }
}
