using MT.Catalogue.Services.Interfaces;
using MT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Catalogue.Services
{
    public class InMemoryCatalogue : ICatalogue 
    { 
        private readonly Dictionary<string, decimal> _prices = new(); 
        public void AddProduct(Product product) 
        { 
            _prices[product.Name] = product.Price; 
        } 
        public bool TryGetPrice(string productName, out decimal price) 
        { 
            return _prices.TryGetValue(productName, out price); 
        } 
    }
}
