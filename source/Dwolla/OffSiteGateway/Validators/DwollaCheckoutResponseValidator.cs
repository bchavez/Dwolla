using FluentValidation;

namespace Dwolla.OffSiteGateway.Validators
{
    public class DwollaCheckoutResponseValidator : AbstractValidator<DwollaCheckoutResponse>
    {
        public DwollaCheckoutResponseValidator()
        {
            RuleFor( cr => cr.Result )
                .Must( r => r == DwollaCheckoutResponseResult.Success )
                .WithMessage( "The checkout request failed. Message from Dwolla's Servers: '{0}'", cr => cr.Message );

            RuleFor( r => r.CheckoutId ).NotEmpty()
                .WithName( "CheckoutResponse.CheckoutId" )
                .When( r=> r.Result == DwollaCheckoutResponseResult.Success);
        }
    }
}