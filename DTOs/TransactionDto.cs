namespace FinTrackAPI.DTOs
{
    public class CreateTransactionDto
    {
        public int AccountId { get; set; }
        public string Type { get; set; } = string.Empty; // "Credit", "Debit", "Transfer"
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
    }
}