namespace RedMango_API.Models.Dto
{
    public class PaymentRequestDTO
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }
}
