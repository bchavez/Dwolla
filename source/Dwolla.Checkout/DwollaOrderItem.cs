using Dwolla.Checkout.Validators;
using FluentValidation.Attributes;

namespace Dwolla.Checkout
{
    [Validator(typeof(DwollaOrderItemValidator))]
    public class DwollaOrderItem
    {
#if DEBUG
        public DwollaOrderItem()
        {
        }
#endif

        public DwollaOrderItem( string name, decimal price, int quantity )
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        /// <summary>Description of the item. Must be 200 characters or less.</summary>
        /// <remarks>Required: Yes</remarks>
        public string Description { get; set; }
        
        /// <summary>Name of the item. Must be 100 characters or less.</summary>
        /// <remarks>Required: Yes</remarks>
        public string Name { get; set; }
        
        /// <summary>Price of the item. Must be $0 or more.</summary>
        /// <remarks>Required: Yes</remarks> 
        public decimal Price { get; set; }
        
        /// <summary>Quantity of the item. Must be 1 or more.</summary>
        /// <remarks>Required: Yes</remarks>
        public int Quantity { get; set; }
    }
}