namespace JARS_API.BusinessModels
{
    public class AccountWithTransactionCount
    {
        public string Id { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int TransactionCount { get; set; }
    }
}
