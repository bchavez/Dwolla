using FluentValidation;

namespace Dwolla.OffSiteGateway.Validators
{
    public class DwollaCheckoutResponseValidator : AbstractValidator<DwollaCheckoutResponse>
    {
        public DwollaCheckoutResponseValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor( cr => cr.Result )
                .Must( r => r == DwollaCheckoutRequestResult.Success )
                .WithMessage( "The checkout request failed. Message from Dwolla's Servers: '{0}'", cr => cr.Message );

            RuleFor( r => r.CheckoutId ).NotEmpty()
                .WithName( "CheckoutResponse.CheckoutId" );
        }
    }
}