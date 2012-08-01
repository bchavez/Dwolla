using FluentValidation;

namespace Dwolla.OffSiteGateway.Validators
{
    public class DwollaPurchaseOrderValidator : AbstractValidator<DwollaPurchaseOrder>
    {
        public DwollaPurchaseOrderValidator()
        {
            RuleFor( po => po.DestinationId ).NotEmpty()
                .WithName("PurchaseOrder.DestinationId");

            RuleFor( po => po.Discount ).LessThanOrEqualTo( 0.00m )
                .WithName("PurchaseOrder.Discount");

            RuleFor( po => po.Shipping ).GreaterThanOrEqualTo( 0.00m )
                .WithName( "PurchaseOrder.Shipping" );

            RuleFor( po => po.Tax ).GreaterThanOrEqualTo( 0.00m )
                .WithName("PurchaseOrder.Tax");

            RuleFor( po => po.OrderItems ).NotNull()
                .Must( x => x.Count >= 1 )
                .SetCollectionValidator( new DwollaOrderItemValidator() )
                .WithName( "PurchaseOrder.OrderItems" );

            RuleFor( po => po.Total ).GreaterThanOrEqualTo( 1.00m )
                .WithName("PurchaseOrder.Total");

            RuleFor( po => po.Notes ).Length( 0, 250 )
                .When( po => po.Notes != null )
                .WithName("PurchaseOrder.Notes");

            RuleFor( po => po.FacilitatorAmount ).GreaterThanOrEqualTo( 0 )
                .Must( HasValidFacilitatorAmount )
                .When( po => po.FacilitatorAmount != null )
                .WithMessage( "Facilitator fee cannot exceed 25% of the total and must be greater than or equal to zero or null." );
        }
        protected virtual bool HasValidFacilitatorAmount(DwollaPurchaseOrder po, decimal? facilitatorAmount )
        {
            const decimal FacilitatorMaxPrecent = 0.25m;

            if( facilitatorAmount == null ) return true;

            if( facilitatorAmount < 0 ) return false;

            var maximumFacilitatorFee = po.Total * FacilitatorMaxPrecent;
            if( facilitatorAmount >= maximumFacilitatorFee ) return false;

            return true;
        }
    }
}