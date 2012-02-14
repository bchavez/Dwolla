using FluentValidation;

namespace Dwolla.OffSiteGateway.Validators
{
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