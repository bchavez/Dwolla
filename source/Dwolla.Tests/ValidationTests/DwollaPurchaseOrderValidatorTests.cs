using System;
using System.Linq;
using Dwolla.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dwolla.Tests.ValidationTests
{
    [TestFixture]
    public class DwollaPurchaseOrderValidatorTests
    {
        private DwollaPurchaseOrderValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaPurchaseOrderValidator();
        }

        [Test]
        public void notes_validation()
        {
            //require length to be less than or equal to 250 chars
            validator.ShouldHaveValidationErrorFor( po => po.Notes,
                                                    "aasdfasddfsadfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfadfasdfasdfasdfasdfadfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdffffffffffff" );

            validator.ShouldNotHaveValidationErrorFor( po => po.Notes, null as string );
            validator.ShouldNotHaveValidationErrorFor( po => po.Notes, "" );
            validator.ShouldNotHaveValidationErrorFor( po => po.Notes, "Foo" );
        }

        [Test]
        public void destination_id_property_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( po => po.DestinationId, "812-111-1111" );

            validator.ShouldHaveValidationErrorFor( po => po.DestinationId, "" );
            validator.ShouldHaveValidationErrorFor( po => po.DestinationId, null as string );
        }

        [Test]
        public void discount_property_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( po => po.Discount, -5.00m );

            validator.ShouldNotHaveValidationErrorFor( po => po.Discount, 0.00m );

            validator.ShouldHaveValidationErrorFor( po => po.Discount, 5.00m );
        }

        [Test]
        public void shipping_property_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( po => po.Shipping, 0.00m );
            validator.ShouldNotHaveValidationErrorFor( po => po.Shipping, 5.00m );

            validator.ShouldHaveValidationErrorFor( po => po.Shipping, -5.00m );
        }

        [Test]
        public void tax_property_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( po => po.Tax, 0.00m );
            validator.ShouldNotHaveValidationErrorFor( po => po.Tax, 5.00m );

            validator.ShouldHaveValidationErrorFor( po => po.Tax, -5.00m );
        }

        [Test]
        public void order_items_should_have_a_child_validator_to_validate_individual_order_items()
        {
            validator.ShouldHaveChildValidator( po => po.OrderItems, typeof(DwollaOrderItemValidator) );
        }

        [Test]
        public void order_items_should_have_at_least_one_order_item()
        {
            validator.ShouldHaveValidationErrorFor( po => po.OrderItems, new DwollaPurchaseOrder() );
        }

        [Test]
        public void order_items_with_at_least_one_order_item_is_okay()
        {
            var goodOrder = new DwollaPurchaseOrder()
                                {
                                    OrderItems = {new DwollaOrderItem("foo", 5.00m, 1)}
                                };
            validator.ShouldNotHaveValidationErrorFor( po => po.OrderItems, goodOrder );            
        }

        [Test]
        public void total_validation()
        {
            var order = new DwollaPurchaseOrder
                            {
                                OrderItems =
                                    {
                                        new DwollaOrderItem("Candy Bar", 25.00m, 1)
                                            {
                                                Description = "Super Expensive Candy Bar"
                                            },
                                            new DwollaOrderItem("Lolly Pop", 30.00m, 2)
                                            {
                                                Description = "Super Expensive Lolly Pop"
                                            },
                                    }
                            };

            validator.ShouldNotHaveValidationErrorFor( po => po.Total, order );

            order.OrderItems.Skip( 1 ).First().Price = -12.49m;
            validator.ShouldHaveValidationErrorFor( po => po.Total, order );
        }

        [Test]
        public void facilitator_validation()
        {
            var order = new DwollaPurchaseOrder
                            {
                                OrderItems =
                                    {
                                        new DwollaOrderItem( price: 25.00m, quantity: 1, name: "Candy Bar" )
                                            {
                                                Description = "Super Expensive Candy Bar"
                                            }
                                    },
                                FacilitatorAmount = 5.00m
                            };

            validator.ShouldNotHaveValidationErrorFor( po => po.FacilitatorAmount, order );

            order.FacilitatorAmount = 6.25m; // max facilitator fee for the total
            validator.ShouldHaveValidationErrorFor( po => po.FacilitatorAmount, order );
        }

        [Test]
        public void ensure_customerinfo_property_has_a_validator()
        {
            validator.ShouldHaveChildValidator( po => po.CustomerInfo, typeof(DwollaCustomerInfoValidator) );
        }

        [Test]
        [Explicit]
        public void error_message_output()
        {
            var order = new DwollaPurchaseOrder
                {
                    CustomerInfo = new DwollaCustomerInfo
                        {
                            FirstName = "Brian",
                            LastName = "Chavez",
                            City = "Beverly Hills",
                            State = "Ca",
                            Zip = "90210",
                            Email = "bchavez-invalid",
                        },
                    OrderItems =
                        {
                            new DwollaOrderItem( price: 25.00m, quantity: 1, name: "Candy Bar" )
                                {
                                    Description = "Super Expensive Candy Bar"
                                }
                        },
                    DestinationId = "123-123-1234",
                    FacilitatorAmount = 5.00m,
                };

            var vr = validator.Validate( order );
            vr.Errors.ToList().ForEach( x => Console.WriteLine( x.ToString() ) );
            vr.IsValid.Should().BeTrue();
        }

    }
}