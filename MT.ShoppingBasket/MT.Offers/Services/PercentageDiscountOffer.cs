using MT.Basket.Models;
using MT.Catalogue.Services.Interfaces;
using MT.Offers.Services.Interfaces;

namespace MT.Offers.Services
{
    public class PercentageDiscountOffer : IOffer
    {
        private readonly string _product;
        private readonly decimal _percentage;

        public string Description => $"{_percentage * 100}% off {_product}";

        public PercentageDiscountOffer(string product, decimal percentage)
        {
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentException("Product name cannot be null or empty.", nameof(product));

            if (percentage <= 0 || percentage > 1)
                throw new ArgumentException("Percentage must be between 0 and 1 (e.g., 0.25 for 25%).", nameof(percentage));

            _product = product;
            _percentage = percentage;
        }

        public decimal CalculateDiscount(ShoppingBasket basket, ICatalogue catalogue)
        {
            try
            {
                if (basket == null)
                    throw new ArgumentNullException(nameof(basket));

                if (catalogue == null)
                    throw new ArgumentNullException(nameof(catalogue));

                var item = basket.Items.FirstOrDefault(i => i.Product.Name == _product);
                if (item == null)
                    return 0;

                if (!catalogue.TryGetPrice(_product, out var price))
                    return 0;

                return item.Quantity * price * _percentage;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to calculate percentage discount for product '{_product}'.", ex);
            }
        }
    }
}
