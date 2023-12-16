using API.Database;
using FluentValidation;

namespace API.DTOs.Employee.Validations;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateProductDto>
{
    private readonly AppDbContext _appDbContext;

    public CreateEmployeeDtoValidator(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        Include(new BaseEmployeeDtoValidator(_appDbContext));
    }
}
