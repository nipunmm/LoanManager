namespace LoanManager.Models
{
    public class Customer
    {

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string IdentityType { get; set; }

        public string IdentityNumber { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
