using System;
using System.Configuration;
using System.Net;
using Dwolla.Checkout.Serialization;
using Dwolla.Checkout.Validators;
using FluentValidation;
using FluentValidation.Attributes;
using RestSharp;

namespace Dwolla.Checkout
{
    [Validator(typeof(DwollaServerCheckoutApiValidator))]
    public class DwollaServerCheckoutApi
    {
        public virtual WebProxy Proxy { get; set; }
        public virtual IValidatorFactory ValidatorFactory { get; set; }

        /// <summary>Dwolla Test Mode Flag. All requests made with this API will enable the testmode flag in the Dwolla request.</summary>
        public virtual bool TestMode { get; private set; }
        /// <summary>Dwolla application key</summary>
        public virtual string AppKey { get; set; }
        /// <summary>Dwolla application secret</summary>
        public virtual string AppSecret { get; set; }

        public const string BaseUrl = "https://www.dwolla.com";
        public const string RequestUrl = "/oauth/rest/offsitegateway/checkouts";
        public static readonly string CheckoutUrl = $"{BaseUrl}/payment/checkout/{{CheckoutId}}";

        private const string DwollaKey = "dwolla_key";
        private const string DwollaSecret = "dwolla_secret";

        public DwollaServerCheckoutApi( string appKey = "", string appSecret = "", bool testMode = false, WebProxy proxy = null)
        {
            this.Proxy = proxy;
            this.TestMode = testMode;
           this.AppKey = !string.IsNullOrWhiteSpace(appKey)
              ? appKey
              : throw new ArgumentException($"The {nameof(appKey)} is null or blank. The {nameof(appKey)} must be specified.");

           this.AppSecret = !string.IsNullOrEmpty(appSecret)
              ? appSecret
              : throw new ArgumentException($"The {nameof(appSecret)} is null or blank. The {nameof(appSecret)} must be specified.");

            this.ValidatorFactory = new AttributedValidatorFactory();
            
            this.ValidatorFactory.GetValidator<DwollaServerCheckoutApi>()
                .ValidateAndThrow( this );
        }


        /// <summary>Executes a POST request to the Off-Site Gateway API. </summary>
        /// <param name="checkoutRequest">A valid Dwolla checkout request.</param>
        /// <returns>Response by Dwolla. It is the callers responsibility to ensure the return object's Result property is 'Success'</returns>
        public virtual DwollaCheckoutResponse SendCheckoutRequest( DwollaCheckoutRequest checkoutRequest )
        {
            this.ValidatorFactory.GetValidator<DwollaServerCheckoutApi>()
                .ValidateAndThrow( this );

            if( string.IsNullOrWhiteSpace(checkoutRequest.ClientId) ){}
                checkoutRequest.ClientId = this.AppKey;
            if( string.IsNullOrWhiteSpace(checkoutRequest.ClientSecret))
                checkoutRequest.ClientSecret = this.AppSecret;

            checkoutRequest.Test = this.TestMode;

            this.ValidatorFactory.GetValidator<DwollaCheckoutRequest>()
                .ValidateAndThrow( checkoutRequest );

            return ExecuteRestRequest( checkoutRequest );
        }

        protected JsonNetSerializer JsonNetSerializer = new JsonNetSerializer();

        protected virtual RestClient GetRestClient()
        {
            var client = new RestClient(BaseUrl)
                {
                    Proxy = this.Proxy,
                };
            client.AddHandler("application/json", this.JsonNetSerializer);
            return client;
        }

        protected virtual RestRequest GetRestRequest(string url)
        {
            return new RestRequest(url)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = this.JsonNetSerializer
                };
        }

        /// <summary>Executes the Dwolla Checkout REST Request. This method can be overridden if you wish to use a different REST library to execute the actual request. </summary>
        protected virtual DwollaCheckoutResponse ExecuteRestRequest( DwollaCheckoutRequest checkoutRequest)
        {
            var client = this.GetRestClient();
            var req = this.GetRestRequest(RequestUrl);
            req.Method = Method.POST;
            req.AddBody(checkoutRequest);

            var res = client.Execute<DwollaCheckoutResponse>( req );

            if( res.ResponseStatus != ResponseStatus.Completed || res.StatusCode != HttpStatusCode.OK )
                return new DwollaCheckoutResponse {Success = false, Message = "Non HTTP status code received."};

            return res.Data;
        }

        [Obsolete( "Use method on response to get the checkout URL: DwollaCheckoutResponse.GetRedirectUrl()" )]
        public virtual string GetCheckoutRedirectUrl(DwollaCheckoutResponse response)
        {
            this.ValidatorFactory.GetValidator<DwollaCheckoutResponse>()
                .ValidateAndThrow( response );

            return CheckoutUrl.Replace( "{CheckoutId}", response.CheckoutId );
        }
        
        public virtual bool VerifyAuthenticity(DwollaCallback receivedCallback)
        {
            this.ValidatorFactory.GetValidator<DwollaServerCheckoutApi>()
                .ValidateAndThrow( this );

            return DwollaSignatureUtil.VerifySignature( this.AppSecret, receivedCallback.Signature, receivedCallback.CheckoutId, receivedCallback.Amount );
        }
        public virtual bool VerifyAuthenticity( DwollaRedirect receivedRedirect )
        {
            this.ValidatorFactory.GetValidator<DwollaServerCheckoutApi>()
                .ValidateAndThrow( this );

            return DwollaSignatureUtil.VerifySignature( this.AppSecret, receivedRedirect.Signature, receivedRedirect.CheckoutId, receivedRedirect.Amount );
        }
    }
}