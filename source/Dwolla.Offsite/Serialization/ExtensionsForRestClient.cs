using RestSharp;

namespace Dwolla.Offsite.Serialization
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