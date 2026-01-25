using MT.Basket;
using MT.Basket.Models;
using MT.Catalogue;
using MT.Catalogue.Services.Interfaces;
using MT.Offers.Services.Interfaces;

namespace MT.Offers
{
    public class BuyNGetCheapestFreeOffer : IOffer
    {
        private readonly List<string> _products;
        private readonly int _n;

        public string Description => $"Buy {_n}, get cheapest free (mixed products)";

        public BuyNGetCheapestFreeOffer(IEnumerable<string> products, int n)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            if (n <= 1)
                throw new ArgumentException("N must be greater than 1 for this offer.", nameof(n));

            _products = products.ToList();
            _n = n;
        }

        public decimal CalculateDiscount(ShoppingBasket basket, ICatalogue catalogue)
        {
            try
            {
                var prices = ExtractEligiblePrices(basket, catalogue);

                if (!HasEnoughItemsForOffer(prices))
                    return 0;

                SortPrices(prices);

                return CalculateCheapestFreeDiscount(prices);
            }
            catch (Exception ex)
            {
                // In a real system, you'd log this exception.
                throw new InvalidOperationException("Failed to calculate BuyNGetCheapestFree discount.", ex);
            }
        }

        private List<decimal> ExtractEligiblePrices(ShoppingBasket basket, ICatalogue catalogue)
        {
            var prices = new List<decimal>();

            try
            {
                foreach (var item in basket.Items)
                {
                    if (!_products.Contains(item.Product.Name))
                        continue;

                    if (!catalogue.TryGetPrice(item.Product.Name, out var price))
                        continue;

                    for (int i = 0; i < item.Quantity; i++)
                        prices.Add(price);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error extracting eligible prices.", ex);
            }

            return prices;
        }

        private bool HasEnoughItemsForOffer(List<decimal> prices)
        {
            return prices.Count >= _n;
        }

        private void SortPrices(List<decimal> prices)
        {
            try
            {
                prices.Sort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to sort prices.", ex);
            }
        }

        private decimal CalculateCheapestFreeDiscount(List<decimal> prices)
        {
            try
            {
                decimal discount = 0;

                for (int i = _n - 1; i < prices.Count; i += _n)
                {
                    discount += prices[i - (_n - 1)];
                }

                return discount;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to calculate cheapest-free discount.", ex);
            }
        }
    }
}
