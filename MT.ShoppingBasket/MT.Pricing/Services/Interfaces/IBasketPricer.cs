using MT.Basket.Models;
using MT.Catalogue.Services.Interfaces;
using MT.Domain.Models;
using MT.Offers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Pricing.Services.Interfaces
{
    public interface IBasketPricer 
    { 
        PricingResult Price(ShoppingBasket basket, ICatalogue catalogue, IEnumerable<IOffer> offers); 
    }
}
