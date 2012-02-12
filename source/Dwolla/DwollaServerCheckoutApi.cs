using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using FluentValidation;
using FluentValidation.Attributes;
using RestSharp;

namespace Dwolla
{
    public class DwollaServerCheckoutApi
    {
        public virtual IValidatorFactory ValidatorFactory { get; set; }

        public virtual bool TestMode { get; private set; }
        public virtual string AppKey { get; set; }
        public virtual string AppSecret { get; set; }

        public const string RequestUrl = "https://www.dwolla.com/payment/request";
        public const string CheckoutUrl = "https://www.dwolla.com/payment/checkout/{CheckoutId}";

        public DwollaServerCheckoutApi() : this( 
            ConfigurationManager.AppSettings["dwolla_key"],
            ConfigurationManager.AppSettings["dwolla_secret"],
            Convert.ToBoolean( ConfigurationManager.AppSettings["dwolla_testmode"] ) )
        {
        }

        public DwollaServerCheckoutApi( string appKey, string appSecret, bool testMode = false )
        {
            this.TestMode = testMode;
            this.AppKey = appKey;
            this.AppSecret = appSecret;

            this.ValidatorFactory = new AttributedValidatorFactory();
        }


        /// <summary>Executes a POST request to the Off-Site Gateway API. </summary>
        /// <param name="checkoutRequest">A valid Dwolla checkout request.</param>
        /// <returns>Response by Dwolla. It is the callers responsibility to ensure the return object's Result property is 'Success'</returns>
        public virtual DwollaCheckoutResponse SendCheckoutRequest( DwollaCheckoutRequest checkoutRequest )
        {
            if( string.IsNullOrWhiteSpace(checkoutRequest.Key) )
                checkoutRequest.Key = this.AppKey;
            if( string.IsNullOrWhiteSpace(checkoutRequest.Secret))
                checkoutRequest.Secret = this.AppSecret;

            checkoutRequest.Test = this.TestMode;

            this.ValidatorFactory.GetValidator<DwollaCheckoutRequest>()
                .ValidateAndThrow( checkoutRequest );

            return ExecuteRestRequest( checkoutRequest );
        }

        /// <summary>Executes the Dwolla Checkout REST Request. This method can be overridden if you wish to use a different REST library to execute the actual request. </summary>
        protected virtual DwollaCheckoutResponse ExecuteRestRequest( DwollaCheckoutRequest checkoutRequest)
        {
            var client = new RestClient();

            var req = new RestRequest( RequestUrl, Method.POST )
            {
                RequestFormat = DataFormat.Json
            }
                .AddBody( checkoutRequest );

            var res = client.Execute<DwollaCheckoutResponse>( req );

            if( res.ResponseStatus != ResponseStatus.Completed || res.StatusCode != HttpStatusCode.OK )
                return new DwollaCheckoutResponse {Result = DwollaCheckoutRequestResult.Failure, Message = "Non HTTP status code received."};

            return res.Data;
        }

        public virtual string GetCheckoutRedirectUrl(DwollaCheckoutResponse response)
        {
            this.ValidatorFactory.GetValidator<DwollaCheckoutResponse>()
                .ValidateAndThrow( response );

            return CheckoutUrl.Replace( "{CheckoutId}", response.CheckoutId );
        }
        
        public virtual bool VerifyCallbackAuthenticity(DwollaCallback receivedCallback)
        {
            return DwollaSignatureUtil.VerifyCallbackSignature( this.AppSecret, receivedCallback.Signature, receivedCallback.CheckoutId, receivedCallback.Amount );
        }
    }

    [Validator(typeof(DwollaCheckoutRequestValidator))]
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

    [Validator( typeof( DwollaCheckoutResponseValidator ) )]
    public class DwollaCheckoutResponse
    {
        public DwollaCheckoutResponse()
        {
            //by default, fail.
            Result = DwollaCheckoutRequestResult.Failure;
        }

        /// <summary>The result of the checkout request.</summary>
        public DwollaCheckoutRequestResult Result { get; set; }

        /// <summary>The CheckoutId generated by Dwolla used for a URL redirect.</summary>
        public string CheckoutId { get; set; }

        /// <summary>The error message if the Result is a Failure.</summary>
        public string Message { get; set; }
    }
    public enum DwollaCheckoutRequestResult
    {
        Success = 0x01,
        Failure
    }

    [Validator( typeof( DwollaPurchaseOrderValidator ) )]
    public class DwollaPurchaseOrder
    {
        public DwollaPurchaseOrder()
        {
            this.OrderItems = new List<DwollaOrderItem>();
        }

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
    }

    [Validator(typeof(DwollaOrderItemValidator))]
    public class DwollaOrderItem
    {
        /// <summary>Description of the item. Must be 200 characters or less.</summary>
        /// <remarks>Required: No</remarks>
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

    public class DwollaCallback
    {
        /// <summary>Amount of the purchase order formatted to 2 decimal places.</summary>
        public decimal Amount { get; set; }
        
        /// <summary>Order ID provided during the request.</summary>
        public string OrderId { get; set; }
        
        /// <summary>Flagged to 'true' if purchase order for testing purposes only, 'false' otherwise.</summary>
        public bool TestMode { get; set; }
        
        /// <summary>Dwolla transaction ID to identify the successful transaction. If in test mode, the transaction ID has a value of 1.</summary>
        public long TransactionId { get; set; }
        
        /// <summary>Unique purchase order ID generated by Off-Site Gateway.</summary>
        public string CheckoutId { get; set; }
        
        /// <summary>Value of 'Completed' if the transaction was successful. If any errors occured, the status will have a value of 'Failed'.</summary>
        public DwollaCallbackStatus Status { get; set; }
        
        /// <summary>HMAC-SHA1 hexadecimal hash of the CheckoutId and amount ampersand separated using the consumer secret of the application as the hash key.</summary>
        public string Signature { get; set; }
    }

    public enum DwollaCallbackStatus
    {
        Completed = 0x01,
        Failed
    }

}