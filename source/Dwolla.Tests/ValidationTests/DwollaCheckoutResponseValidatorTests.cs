using System.Linq;
using Dwolla.OffSiteGateway;
using Dwolla.OffSiteGateway.Validators;
using FluentValidation.TestHelper;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace Dwolla.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaCheckoutResponseValidatorTests
    {
        private DwollaCheckoutResponseValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            this.validator = new DwollaCheckoutResponseValidator();
        }

        [Test]
        public void checkout_result_should_not_pass_validation_if_result_is_a_failure()
        {
            this.validator.ShouldHaveValidationErrorFor( cr => cr.Result, DwollaCheckoutRequestResult.Failure );
        }

        [Test]
        public void fluent_validate_should_display_checkout_result_message_on_failed_checkout_response()
        {
            var result = this.validator.Validate( new DwollaCheckoutResponse { Result = DwollaCheckoutRequestResult.Failure, Message = "Invalid total." } );
            result.IsValid.ShouldBeFalse();
            result.Errors.First().ErrorMessage.ShouldEqual( "The checkout request failed. Message from Dwolla's Servers: 'Invalid total.'" );
            result.Errors.Count.ShouldEqual( 1 );
        }
    }
}