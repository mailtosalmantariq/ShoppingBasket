
MT.ShoppingBasket

A modular, extensible supermarket basket pricing engine built in .NET 10.0. This solution calculates subtotal, discount, and total for any basket of goods using a catalogue and a flexible set of offers.

The component is designed to be:
    Reusable across POS systems, online shops, and internal tools
    Extensible via pluggable offer strategies
    Testable with deterministic pricing logic
    Fair, ensuring maximum discount is always applied

📦 Solution Structure

    MT.ShoppingBasket/
    ├── MT.Domain/            # Core domain models (Product, BasketItem, PricingResult)
    ├── MT.Basket/            # ShoppingBasket implementation
    ├── MT.Catalogue/         # ICatalogue + InMemoryCatalogue
    ├── MT.Offers/            # Offer strategies + IOffer
    ├── MT.Pricing/           # BasketPricer + IBasketPricer
    └── MT.Pricing.Tests/     # NUnit test suite

Each project has a single responsibility and depends only on what it needs, keeping the architecture clean and maintainable.

🚀 How to Run

Clone the repository:
    git clone https://github.com/mailtosalmantariq/ShoppingBasket.git

Open the solution in Visual Studio.
Build the solution.
Run tests using Test Explorer or:
    dotnet test

🧠 Core Concepts

Basket
    A mutable collection of items the customer wants to buy.

Catalogue
    Maps product names to their undiscounted prices.

Offers
    Pricing rules that apply discounts based on basket contents.

BasketPricer
    A pure service that calculates:
        Subtotal
        Discount
        Total

It does not mutate the basket and does not store state.

🎁 Supported Offer Types

Buy X Get Y Free

Example: Buy 2 get 1 free on Baked Beans.
Percentage Discount

Example: 25% off Sardines.
Buy N Get Cheapest Free (Mixed Products)

Example: Buy any 3 shampoos, get the cheapest free.
All offers implement IOffer and can be added without modifying the pricer.

🧪 Test Coverage (NUnit)

The project includes a comprehensive test suite validating:

✔ Assignment Examples
    Basket 1: Baked Beans × 4, Biscuits × 1
    Basket 2: Baked Beans × 2, Biscuits × 1, Sardines × 2
    Shampoo bundle: Buy 3 get cheapest free across mixed sizes

✔ Required Behaviour
    Empty basket returns zero subtotal, discount, and total
    Offers scale correctly (e.g., 3→1 free, 6→2 free, 9→3 free)
    Offers on products not in catalogue produce no discount
    Multiple offers on the same product stack correctly

✔ Example Test Snippet

[Test]
public void Example1_Basket1_ShouldMatchAssignment()
{
    var basket = new ShoppingBasket();
    basket.AddItem(new Product("Baked Beans", 0.99m), 4);
    basket.AddItem(new Product("Biscuits", 1.20m), 1);

    var offers = new List<IOffer>
    {
        new BuyXGetYFreeOffer("Baked Beans", 2, 1),
        new PercentageDiscountOffer("Sardines", 0.25m)
    };

    var result = _pricer.Price(basket, _catalogue, offers);

    Assert.That(result.SubTotal, Is.EqualTo(5.16m));
    Assert.That(result.Discount, Is.EqualTo(0.99m));
    Assert.That(result.Total, Is.EqualTo(4.17m));
}

The full test suite is located in MT.Pricing.Tests under BasketPricerTests.cs.

📊 Example Outputs
    Basket 1
    Baked Beans × 4
    Biscuits × 1
    Subtotal: £5.16Discount: £0.99Total: £4.17

    Basket 2
    Baked Beans × 2
    Biscuits × 1
    Sardines × 2
    Subtotal: £6.96Discount: £0.95Total: £6.01

    Shampoo Bundle
    Large × 3
    Medium × 1
    Small × 2
    Subtotal: £17.00Discount: £5.50Total: £11.50

🧱 Extending the System

To add a new offer:
    Create a class implementing IOffer in MT.Offers.
    Implement your discount logic.
    Add the offer to the list passed to BasketPricer.
    Add corresponding tests in MT.Pricing.Tests.

No changes to the pricer or other offers are required.
