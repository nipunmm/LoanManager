using System.ComponentModel.DataAnnotations;

namespace LoanManager.Models
{
    public class LoanAccount
    {

        public int LoanId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LoanTypeId { get; set; }

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        public int LoanDuration { get; set; }

        public decimal MonthlyRental { get; set; }

        public string CurrentFlow { get; set; } = "New";

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
