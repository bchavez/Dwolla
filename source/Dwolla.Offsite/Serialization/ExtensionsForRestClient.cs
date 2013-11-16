using RestSharp;

namespace Dwolla.Gateway.Serialization
{
    public static class ExtensionsForRestClient
    {
        public static RestRequest WithNewtonsoft( this RestRequest req )
        {
            req.JsonSerializer = new JsonNetSerializer();
            return req;
        }
    }
}