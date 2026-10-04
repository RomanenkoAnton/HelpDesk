using FluentValidation;
using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Validation
{
    public class DepartmentCreateValidation : AbstractValidator<DepartmentCreateDTO>
    {
        public DepartmentCreateValidation()
        {
            RuleFor(model => model.Title).NotEmpty();
            
        }
    }
}
