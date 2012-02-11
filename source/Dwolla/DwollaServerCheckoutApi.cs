using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;

namespace Dwolla
{
    public class DwollaServerCheckoutApi
    {
        public bool TestMode { get; private set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }

        private const string BaseUrl = "https://www.dwolla.com/payment/request";


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
        }

        public bool VerifyCallback(DwollaCallback receivedCallback)
        {
            return DwollaSignatureUtil.VerifyCallbackSignature( this.AppSecret, receivedCallback.Signature, receivedCallback.CheckoutId, receivedCallback.Amount );
        }
    }

    public static class DwollaSignatureUtil
    {
        public static string GenerateSignature( string appKey, string appSecret, string orderId, DateTime utcTimestamp )
        {
            var timestamp = UnixEpochTime( utcTimestamp );

            var signatureData = String.Format( "{0}&{1}&{2}", appKey, timestamp, orderId );

            return GetHMACSHA1InHex( appKey, signatureData );
        }

        public static bool VerifyCallbackSignature( string appSecret, string callbackSignature, string checkoutId, decimal amount )
        {
            var signatureData = String.Format( "{0}&{1}", checkoutId, amount );

            return callbackSignature == GetHMACSHA1InHex( appSecret, signatureData );
        }

        internal static string GetHMACSHA1InHex(string key, string data )
        {
            var hmacKey = Encoding.ASCII.GetBytes( key );

            var signatureStream = new MemoryStream( Encoding.ASCII.GetBytes( data ) );

            var hex = new HMACSHA1( hmacKey ).ComputeHash( signatureStream )
                .Aggregate( new StringBuilder(), ( sb, b ) => sb.AppendFormat( "{0:x2}", b ), sb => sb.ToString() );

            return hex;
        }

        public static long UnixEpochTime( DateTime utcTime )
        {
            TimeSpan timeDifference = utcTime - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return Convert.ToInt64( timeDifference.TotalSeconds );
        }
    }

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