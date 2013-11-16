using System;
using Dwolla.Checkout.Validators;
using FluentValidation.Attributes;

namespace Dwolla.Checkout
{
    [Validator( typeof( DwollaCheckoutRequestValidator ) )]
    public class DwollaCheckoutRequest
    {
        /// <summary>Consumer key for the application.</summary>
        /// <remarks>Required: Yes</remarks>
        public string Key { get; set; }

        /// <summary>Consumer secret for the application.</summary>
        /// <remarks>Required: Yes</remarks>
        public string Secret { get; set; }

        /// <summary>URL to POST the transaction response to after the user authorizes the purchase. If not provided, will default to the Payment Callback URL set for the consumer application. If no default found, results in error.</summary>
        /// <remarks>Required: No</remarks>
        public Uri Callback { get; set; }

        /// <summary>URL to return the user to after they authorize or cancel the purchase. If not provided, will default to the Payment Redirect URL set for the consumer application. If no default found, results in error.</summary>
        /// <remarks>Required: No</remarks>
        public Uri Redirect { get; set; }

        /// <summary>Flag to "true" to enable guest checkout; enabled by default. Guest Checkout enables customers to pay directly from their bank accounts, without having to register or login to a Dwolla account. Requires AllowFundingSources to be set to "true".</summary>
        /// <remarks>Required: No</remarks>
        public bool AllowGuestCheckout { get; set; }

        /// <summary>Flag to "true" allow the user to select funding sources other than Balance, such as ACH (bank account), or FiSync.</summary>
        /// <remarks>Required: No</remarks>
        public bool AllowFundingSources { get; set; }

        /// <summary>Merchant’s order ID to identify the transaction.</summary>
        /// <remarks>Required: No</remarks>
        public string OrderId { get; set; }

        /// <summary>Flag if purchase order is for testing purposes only. Does not affect account balances and no emails are sent. The transaction ID will always be 1 in the responses.</summary>
        /// <remarks>Required: No</remarks> 
        public bool Test { get; set; }

        /// <summary> Purchase order instance.</summary>
        /// <remarks>Required: Yes</remarks> 
        public DwollaPurchaseOrder PurchaseOrder { get; set; }
    }
}