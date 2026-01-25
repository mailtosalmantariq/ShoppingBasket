using MT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Domain
{
    namespace MT.Domain
    {
        public class BasketItem
        {
            public Product Product { get; }
            public int Quantity { get; private set; }

            public BasketItem(Product product, int quantity)
            {
                Product = product;
                Quantity = quantity;
            }

            public void AddQuantity(int amount)
            {
                Quantity += amount;
            }
        }
    }


}
