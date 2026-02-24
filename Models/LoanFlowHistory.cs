namespace LoanManager.Models
{
    public class LoanFlowHistory
    {

        public int HistoryId { get; set; }

        public int LoanId { get; set; }

        public int RoleId { get; set; }

        public string FlowHistory { get; set; }

        public string FlowStatus { get; set; }

        public string Comment { get; set; }

    }
}
