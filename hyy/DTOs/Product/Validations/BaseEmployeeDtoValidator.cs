using API.Database;
using API.Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs.Employee.Validations;

public class BaseEmployeeDtoValidator : AbstractValidator<BaseProductDto>
{
    private readonly AppDbContext _appDbContext;

    public BaseEmployeeDtoValidator(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        RuleFor(x => x.Name)
            .NotNull();

        RuleFor(x => x.Name)
            .MinimumLength(2)
            .MaximumLength(60)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MinimumLength(2)
            .MaximumLength(2000)
            .NotEmpty();
       
        RuleFor(x => x.CategoryId)
            .NotNull()
            .MustAsync(IsExsistingDepartment)
            .WithMessage("Department is not exsisting");

        RuleFor(x => x.Image)
            .NotNull()
            .Must(IsValidImage)
            .WithMessage("Not valid image");

    }

    public async Task<bool> IsExsistingDepartment(int id, CancellationToken arg)
    {
        return await _appDbContext.Categories.FirstOrDefaultAsync(x => x.Id == id) != null;
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file.Length > 2097152000)
        {
            return false;
        }

        return true;
    }






}
