using API.Database.Base;

namespace API.Database.Models;

public class Category  : BaseEntity, IAuditable
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Product> Products  { get; set; }
}
