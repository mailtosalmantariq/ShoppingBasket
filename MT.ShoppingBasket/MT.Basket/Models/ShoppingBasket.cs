using MT.Domain.Models;
using MT.Domain.MT.Domain;

namespace MT.Basket.Models
{
    public class ShoppingBasket
    {
        private readonly List<BasketItem> _items = new();

        public IReadOnlyList<BasketItem> Items => _items;

        public void AddItem(Product product, int quantity = 1)
        {
            var existing = _items.FirstOrDefault(i => i.Product.Name == product.Name);

            if (existing != null)
            {
                existing.AddQuantity(quantity);
            }
            else
            {
                _items.Add(new BasketItem(product, quantity));
            }
        }
    }
}
