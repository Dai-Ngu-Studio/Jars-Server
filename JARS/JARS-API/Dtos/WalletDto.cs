namespace JARS_API.Dtos
{
    public class WalletDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public decimal WalletAmount { get; set; }
        public decimal Percentage { get; set; }
        public string AccountId { get; set; }
    }
}
