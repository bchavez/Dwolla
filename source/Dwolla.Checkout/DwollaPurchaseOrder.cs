using System.Collections.Generic;
using System.Linq;
using Dwolla.Checkout.Validators;
using FluentValidation.Attributes;

namespace Dwolla.Checkout
{
    [Validator( typeof( DwollaPurchaseOrderValidator ) )]
    public class DwollaPurchaseOrder
    {
        public DwollaPurchaseOrder()
        {
            this.OrderItems = new List<DwollaOrderItem>();
            this.Metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// JSON object representing the customer's contact information.
        /// </summary>
        public DwollaCustomerInfo CustomerInfo { get; set; }

        /// <summary>Dwolla key of the Dwolla account receiving the funds.</summary>
        /// <remarks>Required: Yes</remarks>
        /// <example>812-111-1111</example>
        public string DestinationId { get; set; }

        /// <summary>Discount applied to the order. Must be $0.00 or less.</summary>
        /// <remarks>Required: Yes</remarks>
        public decimal Discount { get; set; }

        /// <summary>Shipping total for the order. Must be $0.00 or more.</summary>
        /// <remarks>Required: Yes</remarks>
        public decimal Shipping { get; set; }

        /// <summary>Tax applied the order. Must be $0.00 or more.</summary>
        /// <remarks>Required: Yes</remarks>
        public decimal Tax { get; set; }

        /// <summary>Total of the order. Must be $1.00 or more. Calculated as (item price x quantity) + tax + shipping + discount.</summary>
        /// <remarks>Required: Yes</remarks>
        public virtual decimal Total
        {
            get
            {
                var total = this.OrderItems
                                .Sum( item => item.Price * item.Quantity )
                            + this.Discount
                            + this.Shipping
                            + this.Tax;
                return total;
            }
            private set{} //Not Used but required to deal with bug in Fluent Validation demanding set property be here.
        }

        /// <summary>Amount of the facilitator fee to override. Only applicable if the facilitator fee feature is enabled. If set to 0, facilitator fee is disabled for transaction. Cannot exceed 25% of the total.</summary>
        /// <remarks>Required: No</remarks>
        public decimal? FacilitatorAmount { get; set; }

        /// <summary>List of line items for the purchase order.</summary>
        /// <remarks>Required: Yes</remarks>
        public List<DwollaOrderItem> OrderItems { get; protected set; }

        /// <summary>Note to attach to the transaction. Limited to 250 characters.</summary>
        /// <remarks>Required: No</remarks>
        public string Notes { get; set; }

        /// <summary>
        /// Optional. A JSON object of maximum 10 key-value pairs with which to store metadata in. Keys and values must be strings of max length 255.
        /// </summary>
        /// <remarks>Required: No</remarks>
        public Dictionary<string, string> Metadata { get; set; }
    }
}