using FluentValidation;
using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Validation
{
    public class CategoryUpdateValidation : AbstractValidator<CategoryUpdateDTO>
    {
        public CategoryUpdateValidation()
        {
            RuleFor(model => model.Title).NotEmpty();
            
        }
    }
}
