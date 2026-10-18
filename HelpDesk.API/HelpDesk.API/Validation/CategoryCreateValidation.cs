using FluentValidation;
using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Validation
{
    public class CategoryCreateValidation : AbstractValidator<CategoryCreateDTO>
    {
        public CategoryCreateValidation()
        {
            RuleFor(model => model.Title).NotEmpty();
            
        }
    }
}
