using System;
using Dwolla.Checkout.Validators;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dwolla.Checkout
{
    /// <summary>
    /// Requires allowFundingSources to be set to true. Specify which funding sources are acceptable for this checkout transaction. Multiple sources may be specified in a comma delimited format. Possible values are: 
    /// </summary>
    [Flags]
    public enum FundingSource
    {
        /// <summary>
        /// Display Dwolla Credit, if both the merchant and user support it
        /// </summary>
        Credit,

        /// <summary>
        /// Display all users usable banks
        /// </summary>
        Banks,

        /// <summary>
        /// Display all users usable banks that are FiSync-enabled
        /// </summary>
        Fisync,
        /// <summary>
        /// Display 'credit' and 'fisync' sources
        /// </summary>
        Realtime,

        /// <summary>
        /// Display all
        /// </summary>
        True,

        /// <summary>
        /// Display none
        /// </summary>
        False
    }

    [Validator( typeof( DwollaCheckoutRequestValidator ) )]
    public class DwollaCheckoutRequest
    {
        /// <summary>Consumer key for the application.</summary>
        /// <remarks>Required: Yes</remarks>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>Consumer secret for the application.</summary>
        /// <remarks>Required: Yes</remarks>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Set to true to require the customer to assume the $0.25 Dwolla fee (if applicable) for this transaction. Defaults to false, as the recipient eats the fee by default.
        /// </summary>
        /// <remarks>Required: No</remarks>
        public bool AssumeCosts { get; set; }

        /// <summary>
        /// Set to true to use the Authorize Now / Pay Later flow. Defaults to false, which enables the Pay Now flow.
        /// </summary>
        /// <remarks>Required: No</remarks>
        public bool CheckoutWithApi { get; set; }

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

        /// <summary>
        /// Optional Profile ID. This feature allows a recipient to display a different avatar and name, to collect money on a different person/entity's behalf. Requires special permission to use.
        /// </summary>
        /// <remarks>Required: No</remarks>
        public string ProfileId { get; set; }

        /// <summary>
        /// Requires AllowFundingSources to be set to true. Specify which funding sources are acceptable for this checkout transaction. Multiple sources may be specified in a comma delimited format.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FundingSource? AdditionalFundingSources { get; set; }
    }
}