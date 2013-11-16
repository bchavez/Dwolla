using System;
using Dwolla.Checkout;
using Dwolla.Checkout.Validators;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace Dwolla.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaServerCheckoutApiValidatorTests
    {
        private DwollaServerCheckoutApiValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaServerCheckoutApiValidator();
        }

        [Test]
        public void should_throw_with_null_api_key()
        {
            new Action( () => new DwollaServerCheckoutApi( null, "secret") )
                .ShouldThrow<ValidationException>();
        }
        [Test]
        public void should_throw_with_null_api_secret()
        {
            new Action( () => new DwollaServerCheckoutApi( "key", null) )
                .ShouldThrow<ValidationException>();
        }
    }
}