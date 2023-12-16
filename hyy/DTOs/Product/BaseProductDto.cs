namespace API.DTOs.Employee
{
    public class BaseProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile Image { get; set; }
        public int CategoryId { get; set; }
    }
}

