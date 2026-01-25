namespace MT.Domain.Models
{
    public class PricingResult
    {
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public List<string> AppliedDiscounts { get; set; } = new();
    }
}
