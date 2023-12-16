using API.Database.Models;
using AutoMapper;

namespace API.DTOs.Employee.MappingProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDto, Database.Models.Product>();
            CreateMap<UpdateProductDto, Database.Models.Product>();
        }
    }
}
