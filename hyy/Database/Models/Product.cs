using API.Database.Base;

namespace API.Database.Models;

public class Product : BaseEntity, IAuditable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
}
