using System;
using System.Linq;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Dwolla.Tests
{
    [TestFixture]
    public class DwollaCheckoutRequestTests
    {
        private DwollaCheckoutRequestValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaCheckoutRequestValidator();
        }

        [Test]
        public void valid_key()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Key, "abcdefg" );

            validator.ShouldHaveValidationErrorFor( cr => cr.Key, null as string );
            validator.ShouldHaveValidationErrorFor( cr => cr.Key, string.Empty );
        }
        [Test]
        public void valid_secret()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Secret, "abcdefg" );

            validator.ShouldHaveValidationErrorFor( cr => cr.Secret, null as string );
            validator.ShouldHaveValidationErrorFor( cr => cr.Secret, string.Empty );
        }


        [Test]
        public void callback_url_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Callback, new Uri( "http://www.example.com/callback" ) );

            validator.ShouldHaveValidationErrorFor( cr => cr.Callback, new Uri( "/callback", UriKind.Relative ) );
        }

        [Test]
        public void redirect_url_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.Redirect, new Uri( "http://www.example.com/redirect" ) );

            validator.ShouldHaveValidationErrorFor( cr => cr.Redirect, new Uri( "/redirect", UriKind.Relative ) );
        }

        [Test]
        public void orderid_validation()
        {
            validator.ShouldNotHaveValidationErrorFor( cr => cr.OrderId, "A1B2C3" );
            validator.ShouldNotHaveValidationErrorFor( cr => cr.OrderId, null as string );

            validator.ShouldHaveValidationErrorFor( cr => cr.Secret, "" );
        }

        [Test]
        public void purchase_order_validation()
        {
            validator.ShouldHaveValidationErrorFor( cr => cr.PurchaseOrder, null as DwollaPurchaseOrder );
        }
    }


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
        public void order_items_should_have_at_least_one_order_item()
        {
            validator.ShouldHaveValidationErrorFor( po => po.OrderItems, new DwollaPurchaseOrder() );
        }

        [Test]
        public void order_items_with_at_least_one_order_item_is_okay()
        {
            var goodOrder = new DwollaPurchaseOrder()
                                {
                                    OrderItems = {new DwollaOrderItem()}
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
                                            new DwollaOrderItem
                                                {
                                                    Price = 25.00m,
                                                    Quantity = 1,
                                                    Name = "Candy Bar",
                                                    Description = "Super Expensive Candy Bar"
                                                },
                                            new DwollaOrderItem
                                                {
                                                    Price = 30.00m,
                                                    Quantity = 2,
                                                    Name = "Lolly Pops",
                                                    Description = "Super Expensive Lolly Pops"
                                                }
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
                                        new DwollaOrderItem
                                            {
                                                Price = 25.00m,
                                                Quantity = 1,
                                                Name = "Candy Bar",
                                                Description = "Super Expensive Candy Bar"
                                            }
                                    },
                                FacilitatorAmount = 5.00m
                            };

            validator.ShouldNotHaveValidationErrorFor( po => po.FacilitatorAmount, order );

            order.FacilitatorAmount = 6.25m; // max facilitator fee for the total
            validator.ShouldHaveValidationErrorFor( po => po.FacilitatorAmount, order );
        }

    }

    [TestFixture]
    public class DwollaOrderItemValidationTests
    {
        DwollaOrderItemValidator validator;

        [SetUp]
        public void BeforeEachTest()
        {
            validator = new DwollaOrderItemValidator();
        }

        [Test]
        public void description_should_be_at_least_1_if_not_null()
        {
            validator.ShouldNotHaveValidationErrorFor( o => o.Description, null as string );
            
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
        public void test()
        {
            var foo = new DwollaOrderItem() {Name = "foo", Price = 2.00m, Quantity = 1};

            validator.Validate( foo ).Errors.ToList().ForEach( x => Console.WriteLine( x.ToString() ) );
        }
    }
}