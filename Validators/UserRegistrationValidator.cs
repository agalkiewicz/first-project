using FluentValidation;

public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(4);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Email).EmailAddress().WithName("MailID").WithMessage("{PropertyName} is invalid! Please check!");
        RuleFor(x => x.Password).Equal(z => z.ConfirmPassword).WithMessage("Passwords do not match!");
    }
}
