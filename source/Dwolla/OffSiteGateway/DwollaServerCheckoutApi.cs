using System;
using System.Configuration;
using System.Net;
using FluentValidation;
using FluentValidation.Attributes;
using RestSharp;

namespace Dwolla.OffSiteGateway
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
}