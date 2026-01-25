using MT.Basket.Models;
using MT.Catalogue.Services;
using MT.Domain.Models;
using MT.Offers;
using MT.Offers.Services;
using MT.Offers.Services.Interfaces;
using MT.Pricing.Services;
using NUnit.Framework;

namespace MT.Pricing.Tests.Services
{
    public class AssignmentExampleTests
    {
        private InMemoryCatalogue _catalogue;
        private BasketPricer _pricer;

        [SetUp]
        public void Setup()
        {
            _catalogue = new InMemoryCatalogue();
            _pricer = new BasketPricer();

            // Catalogue from assignment
            _catalogue.AddProduct(new Product("Baked Beans", 0.99m));
            _catalogue.AddProduct(new Product("Biscuits", 1.20m));
            _catalogue.AddProduct(new Product("Sardines", 1.89m));
            _catalogue.AddProduct(new Product("Shampoo (Small)", 2.00m));
            _catalogue.AddProduct(new Product("Shampoo (Medium)", 2.50m));
            _catalogue.AddProduct(new Product("Shampoo (Large)", 3.50m));
        }

        // -------------------------------------------------------------
        // Example 1 from assignment
        // -------------------------------------------------------------
        [Test]
        public void Example1_Basket1_ShouldMatchAssignment()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 4);
            basket.AddItem(new Product("Biscuits", 1.20m), 1);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1),
                new PercentageDiscountOffer("Sardines", 0.25m)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.SubTotal, Is.EqualTo(5.16m));
                Assert.That(result.Discount, Is.EqualTo(0.99m));
                Assert.That(result.Total, Is.EqualTo(4.17m));
            }
        }

        // -------------------------------------------------------------
        // Example 2 from assignment
        // -------------------------------------------------------------
        [Test]
        public void Example2_Basket2_ShouldMatchAssignment()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 2);
            basket.AddItem(new Product("Biscuits", 1.20m), 1);
            basket.AddItem(new Product("Sardines", 1.89m), 2);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1),
                new PercentageDiscountOffer("Sardines", 0.25m)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            Assert.That(result.SubTotal, Is.EqualTo(6.96m));
            Assert.That(result.Discount, Is.EqualTo(0.945m).Within(0.0001m));
            Assert.That(result.Total, Is.EqualTo(6.015m).Within(0.0001m));
        }

        // -------------------------------------------------------------
        // Shampoo Example: Buy 3 get cheapest free (mixed products)
        // -------------------------------------------------------------
        [Test]
        public void Example3_ShampooMixedOffer_ShouldMatchAssignment()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Shampoo (Large)", 3.50m), 3);
            basket.AddItem(new Product("Shampoo (Medium)", 2.50m), 1);
            basket.AddItem(new Product("Shampoo (Small)", 2.00m), 2);

            var offers = new List<IOffer>
            {
                new BuyNGetCheapestFreeOffer(
                    new [] { "Shampoo (Large)", "Shampoo (Medium)", "Shampoo (Small)" },
                    3)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.SubTotal, Is.EqualTo(17.00m));
                Assert.That(result.Discount, Is.EqualTo(5.50m));
                Assert.That(result.Total, Is.EqualTo(11.50m));
            }
        }

        // -------------------------------------------------------------
        // Required by spec: Empty basket
        // -------------------------------------------------------------
        [Test]
        public void EmptyBasket_ShouldReturnZeroes()
        {
            var basket = new ShoppingBasket();
            var offers = new List<IOffer>();

            var result = _pricer.Price(basket, _catalogue, offers);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.SubTotal, Is.EqualTo(0m));
                Assert.That(result.Discount, Is.EqualTo(0m));
                Assert.That(result.Total, Is.EqualTo(0m));
            }
        }

        // -------------------------------------------------------------
        // Buy X Get Y Free scaling: 3→2, 6→4, 9→6
        // -------------------------------------------------------------
        [Test]
        public void Buy2Get1Free_ShouldGiveCorrectDiscount_For3Items()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 3);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            Assert.That(result.Discount, Is.EqualTo(0.99m)); // 1 free
        }

        [Test]
        public void Buy2Get1Free_ShouldGiveCorrectDiscount_For6Items()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 6);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            Assert.That(result.Discount, Is.EqualTo(1.98m)); // 2 free
        }

        [Test]
        public void Buy2Get1Free_ShouldGiveCorrectDiscount_For9Items()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 9);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            Assert.That(result.Discount, Is.EqualTo(2.97m)); // 3 free
        }

        // -------------------------------------------------------------
        // Offer on product NOT in catalogue
        // -------------------------------------------------------------
        [Test]
        public void OfferOnProductNotInCatalogue_ShouldGiveNoDiscount()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Biscuits", 1.20m), 2);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("NonExistentProduct", 2, 1)
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Discount, Is.EqualTo(0m));
                Assert.That(result.AppliedDiscounts.Count, Is.EqualTo(0));
            }
        }

        // -------------------------------------------------------------
        // Multiple offers on the SAME product
        // -------------------------------------------------------------
        [Test]
        public void MultipleOffersOnSameProduct_ShouldApplyAllDiscounts()
        {
            var basket = new ShoppingBasket();
            basket.AddItem(new Product("Baked Beans", 0.99m), 3);

            var offers = new List<IOffer>
            {
                new BuyXGetYFreeOffer("Baked Beans", 2, 1),   // 1 free → 0.99
                new PercentageDiscountOffer("Baked Beans", 0.10m) // 10% off → 3 * 0.99 * 0.10 = 0.297
            };

            var result = _pricer.Price(basket, _catalogue, offers);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Discount, Is.EqualTo(0.99m + 0.297m).Within(0.0001m));
                Assert.That(result.AppliedDiscounts.Count, Is.EqualTo(2));
            }
        }

    }
}
