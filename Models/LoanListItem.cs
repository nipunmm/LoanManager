using System;

namespace LoanManager.Models
{
    public class LoanListItem
    {
        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? IdentityNumber { get; set; }
        public int LoanTypeId { get; set; }
        public string? LoanTypeName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanDuration { get; set; }
        public decimal MonthlyRental { get; set; }
        public string? CurrentFlow { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
