using API.Database;
using API.Database.Models;
using API.DTOs.Category;
using API.DTOs.Employee;
using API.DTOs.Employee.Validations;
using API.Services.abstracts;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(e => e.Id == id);
            if (product == null) { return NotFound(); }

            return Ok(product);
        }

        [HttpGet("get_all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery(Name = "name")] string? name,
            [FromQuery(Name = "min_price")] double? minPrice,
            [FromQuery(Name = "max_price")] double? maxPrice,
            [FromQuery(Name = "category_id")] int? categoryId,
            [FromQuery(Name = "sort")] string? sortField,
            [FromQuery(Name = "desc")] bool desc = false)
        {
            var query = _appDbContext.Products.AsQueryable();
            query = (!string.IsNullOrEmpty(name) ? query.Where(e => e.Name.ToLower().StartsWith(name.ToLower())) : query);
            query = (minPrice != null ? query.Where(e => e.Price >= minPrice) : query);
            query = (maxPrice != null ? query.Where(e => e.Price <= maxPrice) : query);
            query = (categoryId != null ? query.Where(e => e.CategoryId == categoryId) : query);

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "name":
                        query = desc ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name);
                        break;
                    default:
                        query = query.OrderBy(e => e.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }


            var list = await query.ToListAsync();

            return Ok(list);
        }


        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromForm] CreateProductDto productDto)
        {
            var validations = await new CreateEmployeeDtoValidator(_appDbContext).ValidateAsync(productDto);
            if (validations.Errors.Any()) { return BadRequest(validations.Errors.Any()); }

            var product = _mapper.Map<Product>(productDto);
            product.Image = productDto.Image.SaveFile(_webHostEnvironment.WebRootPath, "uploads/images");

            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();

            return Created(nameof(Get), new { id = product.Id });
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromForm] UpdateProductDto productDto)
        {
            var validations = await new UpdateProductDtoValidator(_appDbContext).ValidateAsync(productDto);
            if (validations.Errors.Any()) { return BadRequest(validations.Errors); }

            var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) { return NotFound(); }

            _mapper.Map(productDto, product);

            if (product.Image != null)
            {
                productDto.Image.RemoveFile(_webHostEnvironment.WebRootPath, "uploads/images", product.Image);
                product.Image = productDto.Image.SaveFile(_webHostEnvironment.WebRootPath, "uploads/images");
            }

            _appDbContext.Products.Update(product);

            await _appDbContext.SaveChangesAsync();
            return Ok(new { id = product.Id, name = product.Name });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null) { return NotFound(); }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            CustomFileService.RemoveFile(_webHostEnvironment.WebRootPath, "uploads/images", product.Image);
            return NoContent();
        }

        [HttpPost("category/add")]
        public async Task<IActionResult> AddCategory(CategoryAddDto categoryAddDto)
        {
            Category category = new Category();
            category.Name = categoryAddDto.Name;

            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("categories/get")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _appDbContext.Categories.ToListAsync();
            return Ok(categories);
        }

    }
}
