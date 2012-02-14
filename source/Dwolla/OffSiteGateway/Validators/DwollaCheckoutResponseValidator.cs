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

    public class DwollaServerCheckoutApiValidator : AbstractValidator<DwollaServerCheckoutApi>
    {
        public DwollaServerCheckoutApiValidator()
        {
            RuleFor( s => s.AppKey ).NotEmpty()
                .WithName( "ServerCheckoutApi.AppKey" )
                .WithMessage( "Ensure appSettings/'dwolla_key' is present in your web.config/app.config; or that you pass a valid appKey argument into the 'DwollaServerCheckoutApi' constructor." );

            RuleFor( s => s.AppSecret ).NotEmpty()
                .WithName( "ServerCheckoutApi.AppSecret" )
                .WithMessage( "Ensure appSettings/'dwolla_secret' is present in your web.config/app.config; or that you pass a valid appSecret argument into the 'DwollaServerCheckoutApi' constructor." );
        }
    }
}