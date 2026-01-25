using MT.Basket.Models;
using MT.Catalogue.Services.Interfaces;
using MT.Offers.Services.Interfaces;

namespace MT.Offers.Services
{
    public class BuyXGetYFreeOffer : IOffer
    {
        private readonly string _product;
        private readonly int _x;
        private readonly int _y;

        public string Description => $"Buy {_x} get {_y} free on {_product}";

        public BuyXGetYFreeOffer(string product, int x, int y)
        {
            if (string.IsNullOrWhiteSpace(product))
                throw new ArgumentException("Product name cannot be null or empty.", nameof(product));

            if (x <= 0 || y <= 0)
                throw new ArgumentException("X and Y must be greater than zero.");

            _product = product;
            _x = x;
            _y = y;
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

                int groupSize = _x + _y;
                if (groupSize <= 0)
                    throw new InvalidOperationException("Invalid offer configuration: group size cannot be zero or negative.");

                int freeItems = (item.Quantity / groupSize) * _y;

                return freeItems * price;
            }
            catch (Exception ex)
            {
                // In a real system, you'd log this exception.
                throw new InvalidOperationException($"Failed to calculate discount for product '{_product}'.", ex);
            }
        }
    }
}
