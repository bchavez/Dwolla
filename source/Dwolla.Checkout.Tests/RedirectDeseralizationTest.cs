using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using NUnit.Framework;
using Moq;
using Newtonsoft.Json;
using ObjectDumper;

namespace Dwolla.Checkout.Tests
{


    public class TestController : Controller
    {
        [HttpGet]
        public void DwollaRedirect(DwollaRedirect redirect)
        {
        }
    }

    [TestFixture]
    public class RedirectDeseralizationTest
    {
        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                    {
                        controller = "Home",
                        action = "Index",
                        id = UrlParameter.Optional
                    }
            );
        }

        private T GetModelFor<T>(string url)
        {
            RouteTable.Routes.Clear();
            RegisterRoutes(RouteTable.Routes);

            var httpContext = MvcMockHelpers.MockHttpContext(url);

            var routeData = RouteTable.Routes.GetRouteData(httpContext);

            var controller = new TestController();
            controller.SetMockControllerContext(httpContext, routeData, RouteTable.Routes);

            var queryStringValueProvider = new QueryStringValueProvider(controller.ControllerContext);

            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(T));

            var bindingContext = new ModelBindingContext
            {
                ModelName = null,
                ValueProvider = queryStringValueProvider,
                ModelMetadata = modelMetadata
            };

            var binder = ModelBinders.Binders.GetBinder(typeof(DwollaRedirect));
            return (T)binder.BindModel(controller.ControllerContext, bindingContext);
        }

        [Test]
        public void SuccessRedirect()
        {
            var successRedirect =
                "~/Test/DwollaRedirect?signature=c50e4b2a4c50ad795c3ee31370aec1a563565aa4&orderId=&amount=0.01&checkoutId=f32b1e55-9612-4b6d-90f9-1c1519e588da&status=Completed&clearingDate=8/28/2012%203:17:18%20PM&transaction=1312616&postback=success";

            var redirect = GetModelFor<DwollaRedirect>(successRedirect);

            redirect.Signature.Should().Be("c50e4b2a4c50ad795c3ee31370aec1a563565aa4");
            redirect.OrderId.Should().BeNull();
            redirect.Amount.Should().Be(0.01m);
            redirect.CheckoutId.Should().Be("f32b1e55-9612-4b6d-90f9-1c1519e588da");
            redirect.Status.Should().Be(DwollaStatus.Completed);
            redirect.Transaction.Should().Be(1312616);
            redirect.Postback.Should().Be(DwollaPostbackStatus.Success);
        }

        [Test]
        public void FailureRedirect()
        {
            var failureRedirect =
                "~/Test/DwollaRedirect?checkoutId=694e6bcb-349f-451d-bf5d-8aa16530c960&error=failure&error_description=There+are+insufficient+funds+for+this+transaction";
            var redirect = GetModelFor<DwollaRedirect>(failureRedirect);

            redirect.CheckoutId.Should().Be("694e6bcb-349f-451d-bf5d-8aa16530c960");
            redirect.Error_Description.Should().Be("There+are+insufficient+funds+for+this+transaction");
        }
    }
}