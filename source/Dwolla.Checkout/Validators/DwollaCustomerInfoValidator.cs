using FluentValidation;

namespace Dwolla.Gateway.Validators
{
    public class DwollaCustomerInfoValidator : AbstractValidator<DwollaCustomerInfo>
    {
        public DwollaCustomerInfoValidator()
        {
            RuleFor( ci => ci.FirstName ).NotEmpty()
                .WithName( "CustomerInfo.FirstName" );

            RuleFor( ci => ci.LastName ).NotEmpty()
                .WithName( "CustomerInfo.LastName" );

            RuleFor( ci => ci.Email )
                .NotEmpty()
                .EmailAddress()
                .WithName( "CustomerInfo.Email" );

            RuleFor( ci => ci.City ).NotEmpty()
                .WithName( "CustomerInfo.City" );

            RuleFor( ci => ci.State ).NotEmpty();
            RuleFor( ci => ci.State ).Length( 2 );
            RuleFor( ci => ci.State ).Must( s => s == s.ToUpperInvariant() )
                .Unless( s =>  s.State == null )
                .WithMessage( "'{PropertyName}' must be a two letter state abbreviation in upper case.", i => i.State )
                .WithName( "CustomerInfo.State" );

            RuleFor( ci => ci.Zip )
                .NotEmpty()
                .Matches( @"\A[0-9]{5}\z" )
                .WithName( "CustomerInfo.Zip" );
        }
    }
}