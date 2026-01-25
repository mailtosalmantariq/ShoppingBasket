using System;
using System.Collections.Generic;
using System.Text;

namespace MT.Catalogue.Services.Interfaces
{
    public interface ICatalogue 
    { 
        bool TryGetPrice(string productName, out decimal price); 
    }
}
