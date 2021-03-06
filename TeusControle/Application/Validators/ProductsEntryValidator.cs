using FluentValidation;
using TeusControle.Domain.Models;

namespace TeusControle.Application.Validators
{
    /// <summary>
    /// Validação para objeto do tipo usuário
    /// </summary>
    public class ProductsEntryValidator : AbstractValidator<ProductEntries>
    {
        public ProductsEntryValidator()
        {
            RuleFor(c => c.Amount)
                .NotEmpty().WithMessage("Please enter the amount.")
                .NotNull().WithMessage("Please enter the amount.");
        }
    }
}
