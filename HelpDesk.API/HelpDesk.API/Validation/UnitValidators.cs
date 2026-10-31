using FluentValidation;
using HelpDesk.API.Data.DTO;


namespace HelpDesk.API.Validators
{
    public class UnitCreateValidator : AbstractValidator<UnitCreateDTO>
    {
        public UnitCreateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }

    public class UnitUpdateValidator : AbstractValidator<UnitUpdateDTO>
    {
        public UnitUpdateValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.DepartmentId).GreaterThan(0);
        }
    }
}