using MT.Basket.Models;
using MT.Catalogue;
using MT.Catalogue.Services.Interfaces;
using MT.Domain.Models;
using MT.Offers;
using MT.Offers.Services.Interfaces;
using MT.Pricing.Services.Interfaces;

namespace MT.Pricing
{
    public class BasketPricer : IBasketPricer
    {
        public PricingResult Price(ShoppingBasket basket, ICatalogue catalogue, IEnumerable<IOffer> offers)
        {
            var result = new PricingResult();

            // Subtotal
            foreach (var item in basket.Items)
            {
                if (catalogue.TryGetPrice(item.Product.Name, out var price))
                {
                    result.SubTotal += price * item.Quantity;
                }
            }

            // Discounts
            foreach (var offer in offers)
            {
                var discount = offer.CalculateDiscount(basket, catalogue);
                if (discount > 0)
                {
                    result.Discount += discount;
                    result.AppliedDiscounts.Add(offer.Description);
                }
            }

            // Total
            result.Total = Math.Max(0, result.SubTotal - result.Discount);

            return result;
        }
    }
}
