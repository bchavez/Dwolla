using FluentValidation;

namespace Dwolla.Checkout.Validators
{
    public class DwollaCheckoutRequestValidator : AbstractValidator<DwollaCheckoutRequest>
    {
        public DwollaCheckoutRequestValidator()
        {
            RuleFor( cr => cr.Key ).NotEmpty()
                .WithName( "CheckoutRequest.Key" );

            RuleFor( cr => cr.Secret ).NotEmpty()
                .WithName("CheckoutRequest.Secret");

            RuleFor( cr => cr.Callback )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Callback == null )
                .WithMessage( "The CheckoutRequest.Callback URL must be absolute and a valid http/https URL" );

            RuleFor( cr => cr.Redirect )
                .Must( uri => uri.IsAbsoluteUri )
                .Unless( cr => cr.Redirect == null )
                .WithMessage( "The CheckoutRequest.Redirect URL must be absolute and a valid http/https URL." );

            //allow funding sources must be true if allow guest checkout is enabled.
            RuleFor( cr => cr.AllowFundingSources )
                .Equal(true)
                .When( cr => cr.AllowGuestCheckout )
                .WithMessage( "The 'AllowFundingSources' should be 'true' when 'AllowGuestCheckout' is enabled. See off-site gateway documentation for more information." );
            

            RuleFor( cr => cr.OrderId ).Length( 1, 100 )
                .Unless( cr => cr.OrderId == null );

            RuleFor( cr => cr.PurchaseOrder ).NotNull()
                .SetValidator( new DwollaPurchaseOrderValidator() );
        }
    }
}