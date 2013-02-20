using System;
using System.Linq;
using Dwolla.OffSiteGateway;
using Dwolla.OffSiteGateway.Validators;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dwolla.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaCustomerInfoValidatorTests
    {
        DwollaCustomerInfoValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaCustomerInfoValidator();
        }

        [Test]
        public void firstname_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( i => i.FirstName, string.Empty );
            validator.ShouldHaveValidationErrorFor( o => o.FirstName, null as string );
        }

        [Test]
        public void last_name_proeprty_validation()
        {
            validator.ShouldHaveValidationErrorFor( ci => ci.LastName, null as string );
            validator.ShouldHaveValidationErrorFor( ci => ci.LastName, string.Empty );
        }

        [Test]
        public void email_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( ci => ci.Email, null as string );
            validator.ShouldHaveValidationErrorFor( ci => ci.Email, string.Empty );
            validator.ShouldHaveValidationErrorFor( ci => ci.Email, "ffooffo" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Email, "fwe@fa" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Email, "test@.com" );
            validator.ShouldNotHaveValidationErrorFor( ci => ci.Email, "test@test.com" );
        }

        [Test]
        public void city_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( ci => ci.City, null as string );
            validator.ShouldHaveValidationErrorFor( ci => ci.City, string.Empty );
        }

        [Test]
        public void state_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( ci => ci.State, null as string );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, string.Empty );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, "c" );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, "C" );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, "ca" );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, "cA" );
            validator.ShouldHaveValidationErrorFor( ci => ci.State, "CAX" );
            validator.ShouldNotHaveValidationErrorFor( ci => ci.State, "CA" );
        }

        [Test]
        public void zipcode_property_validation()
        {
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, null as string );
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, string.Empty );

            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, "abc" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, "0" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, "00" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, "000" );
            validator.ShouldHaveValidationErrorFor( ci => ci.Zip, "0000" );

            validator.ShouldNotHaveValidationErrorFor( ci => ci.Zip, "90210" );
        }


        [Test]
        [Explicit]
        public void test()
        {
            var foo = new DwollaCustomerInfo(){ State = "Ca", FirstName = "Brian", LastName = "Chavez", City = "None", Email = "bchavez@foo.com", Zip = "90210"};

            validator.Validate( foo ).Errors.ToList().ForEach( x => Console.WriteLine( x.ToString() ) );
        }
    }
}