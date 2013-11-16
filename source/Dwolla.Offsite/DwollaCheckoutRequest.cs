using System;
using Dwolla.Offsite.Validators;
using FluentValidation.Attributes;

namespace Dwolla.Offsite
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

        /// <summary>URL to POST the transaction response after the user authorizes the purchase. If not provided, will default to registered Payment Callback URL.</summary>
        /// <remarks>Required: No</remarks>
        public Uri Callback { get; set; }

        /// <summary>URL to return the user to after they authorize the purchase. If not provided, will default to registered Payment Redirect URL. If no default found, results in error.</summary>
        /// <remarks>Required: No</remarks>
        public Uri Redirect { get; set; }

        /// <summary>Flag to allow guest checkout and bank-funded payments.</summary>
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