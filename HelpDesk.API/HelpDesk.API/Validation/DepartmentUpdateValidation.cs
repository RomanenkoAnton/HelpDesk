using FluentValidation;
using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Validation
{
    public class DepartmentUpdateValidation : AbstractValidator<DepartmentUpdateDTO>
    {
        public DepartmentUpdateValidation()
        {
            RuleFor(model => model.Title).NotEmpty();
            
        }
    }
}
