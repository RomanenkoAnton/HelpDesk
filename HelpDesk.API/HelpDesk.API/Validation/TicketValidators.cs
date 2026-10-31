using FluentValidation;
using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Validators
{
    public class TicketCreateValidator : AbstractValidator<TicketCreateDTO>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.Model).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DepartmentId).GreaterThan(0);

            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleFor(x => x.AssignedEmployee).MaximumLength(200);
            RuleFor(x => x.IssuedTo).MaximumLength(200);

            RuleFor(x => x.ReturnedFromSZIDate)
                .GreaterThanOrEqualTo(x => x.SentToSZIDate)
                .When(x => x.ReturnedFromSZIDate.HasValue && x.SentToSZIDate.HasValue)
                .WithMessage("Дата возврата из СЗИ не может быть раньше даты отправки");
        }
    }

    public class TicketUpdateValidator : AbstractValidator<TicketUpdateDTO>
    {
        public TicketUpdateValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Model).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DepartmentId).GreaterThan(0);

            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleFor(x => x.AssignedEmployee).MaximumLength(200);
            RuleFor(x => x.IssuedTo).MaximumLength(200);

            RuleFor(x => x.ReturnedFromSZIDate)
                .GreaterThanOrEqualTo(x => x.SentToSZIDate)
                .When(x => x.ReturnedFromSZIDate.HasValue && x.SentToSZIDate.HasValue)
                .WithMessage("Дата возврата из СЗИ не может быть раньше даты отправки");
        }
    }
}