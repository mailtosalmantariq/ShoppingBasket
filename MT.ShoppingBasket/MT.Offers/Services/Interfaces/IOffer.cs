using MT.Basket.Models;
using MT.Catalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Offers.Services.Interfaces
{
    public interface IOffer 
    { 
        decimal CalculateDiscount(ShoppingBasket basket, ICatalogue catalogue); 
        string Description { get; } 
    }
}
