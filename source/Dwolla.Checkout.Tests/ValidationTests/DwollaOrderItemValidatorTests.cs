using System;
using System.Linq;
using Dwolla.Checkout.Validators;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dwolla.Checkout.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaOrderItemValidatorTests
    {
        DwollaOrderItemValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaOrderItemValidator();
        }

        [Test]
        public void description_should_be_at_least_1()
        {
            validator.ShouldHaveValidationErrorFor( o => o.Description, null as string );
            
            validator.ShouldHaveValidationErrorFor( o => o.Description, string.Empty );
        }

        [Test]
        public void name_property_validation()
        {
            //100 chars, 101 chars
            validator.ShouldNotHaveValidationErrorFor( o => o.Name, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" );
            
            validator.ShouldHaveValidationErrorFor( o => o.Name, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" );
            validator.ShouldHaveValidationErrorFor( o => o.Name, null as string );
            validator.ShouldHaveValidationErrorFor( o => o.Name, "" );
        }

        [Test]
        public void price_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( o => o.Price, -1.00m );
            validator.ShouldNotHaveValidationErrorFor( o => o.Price, 0.00m );
        }

        [Test]
        public void quantity_property_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( o => o.Quantity, 1 );

            validator.ShouldHaveValidationErrorFor( o => o.Quantity, 0 );
        }


        
        [Test]
        [Explicit]
        public void message_test()
        {
            var foo = new DwollaOrderItem("foo", 2.00m, 0);

            validator.Validate( foo ).Errors.ToList().ForEach( x => Console.WriteLine( x.ToString() ) );
        }
    }
}