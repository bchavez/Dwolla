using System.Collections.Generic;
using FluentValidation;

namespace Dwolla.Checkout.Validators
{
    public class DwollaOrderItemValidator : AbstractValidator<DwollaOrderItem>
    {
        public DwollaOrderItemValidator()
        {
            RuleFor( item => item.Description )
                .NotNull()
                .Length( 1, 200 );

            RuleFor( item => item.Name )
                .NotNull()
                .Length( 1, 100 )
                .WithName("OrderItem.Name");

            RuleFor( item => item.Price ).GreaterThanOrEqualTo( 0.00m )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to ${ComparisonValue}.", item => item.Name );

            RuleFor( item => item.Quantity ).GreaterThanOrEqualTo( 1 )
                .WithMessage( "The 'OrderItem.{PropertyName}' for '{0}' must be greater than or equal to {ComparisonValue}.", item => item.Name );
                
        }
    }
    
    public class DwollaMetadataValidator : AbstractValidator<KeyValuePair<string, string>>
    {
        public DwollaMetadataValidator()
        {
            RuleFor(kv => kv.Key)
                .NotEmpty()
                .Length(1, 255);
            RuleFor(kv => kv.Value)
                .NotEmpty()
                .Length(1, 255);
        }
    }
}