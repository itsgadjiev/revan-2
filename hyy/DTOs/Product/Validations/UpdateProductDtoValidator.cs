using API.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs.Employee.Validations;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    private readonly AppDbContext _appDbContext;

    public UpdateProductDtoValidator(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        Include(new BaseEmployeeDtoValidator(_appDbContext));
    }

    public async Task<bool> IsExsistingProduct(int emplyeeId, CancellationToken arg)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == emplyeeId) != null;
    }
}
