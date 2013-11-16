using RestSharp;

namespace Dwolla.Serialization
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