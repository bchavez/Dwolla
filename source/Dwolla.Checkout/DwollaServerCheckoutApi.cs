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
        private readonly WebProxy proxy;
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

        /// <summary>
        /// This constructor will retrieve <b>AppKey</b> and <b>AppSecret</b> from your web.config/appSettings location.
        /// AppKey = AppSettings["dwolla_key"] (required),
        /// AppSecret = AppSettings["dwolla_secret"] (required),
        /// TestMode = AppSettings["dwolla_testmode"] (optional) default false.
        /// </summary>
        /// <example>
        ///   &lt;appSettings&gt;
        ///      &lt;add key="dwolla_key" value="...key..."/&gt;
        ///      &lt;add key="dwolla_secret" value="...secret..."/&gt;
        ///      &lt;add key="dwolla_testmode" value="true|false"/&gt;
        ///   &lt;appSettings&gt;
        /// </example>
        public DwollaServerCheckoutApi() : this( 
            ConfigurationManager.AppSettings[DwollaKey],
            ConfigurationManager.AppSettings[DwollaSecret],
            Convert.ToBoolean( ConfigurationManager.AppSettings["dwolla_testmode"] ?? "false" ) )
        {
        }

        private const string DwollaKey = "dwolla_key";
        private const string DwollaSecret = "dwolla_secret";

        public DwollaServerCheckoutApi( string appKey = "", string appSecret = "", bool testMode = false, WebProxy proxy = null)
        {
            this.proxy = proxy;
            this.TestMode = testMode;
            this.AppKey = !string.IsNullOrWhiteSpace(appKey) ? appKey : ConfigurationManager.AppSettings[DwollaKey];
            this.AppSecret = !string.IsNullOrEmpty(appSecret) ? appSecret : ConfigurationManager.AppSettings[DwollaSecret];

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

        /// <summary>Executes the Dwolla Checkout REST Request. This method can be overridden if you wish to use a different REST library to execute the actual request. </summary>
        protected virtual DwollaCheckoutResponse ExecuteRestRequest( DwollaCheckoutRequest checkoutRequest)
        {
            var client = new RestClient(BaseUrl)
                {
                    Proxy = this.proxy
                };

            var req = new RestRequest( RequestUrl, Method.POST )
                {
                    RequestFormat = DataFormat.Json
                }
                .WithNewtonsoft()
                .AddBody( checkoutRequest );

            var res = client.Execute<DwollaCheckoutResponse>( req );

            if( res.ResponseStatus != ResponseStatus.Completed || res.StatusCode != HttpStatusCode.OK )
                return new DwollaCheckoutResponse {Result = DwollaCheckoutResponseResult.Failure, Message = "Non HTTP status code received."};

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