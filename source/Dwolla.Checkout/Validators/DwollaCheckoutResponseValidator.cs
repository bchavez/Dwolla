using FluentValidation;

namespace Dwolla.Checkout.Validators
{
    public class DwollaCheckoutResponseValidator : AbstractValidator<DwollaCheckoutResponse>
    {
        public DwollaCheckoutResponseValidator()
        {
            RuleFor( cr => cr.Success )
                .Must( r => r == true)
                .WithMessage( "The checkout request failed. Message from Dwolla's Servers: '{0}'", cr => cr.Message );

            RuleFor( r => r.CheckoutId ).NotNull().NotEmpty()
                .WithName( "CheckoutResponse.CheckoutId" )
                .When( r => r.Success == true);
        }
    }
}