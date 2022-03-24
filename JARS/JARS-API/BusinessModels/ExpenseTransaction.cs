namespace JARS_API.BusinessModels;

public class ExpenseTransaction
{
    public int WalletId { get; set; }
    public decimal Amount { get; set; }
    public String? NoteComment { get; set; }
    public String? NoteImage { get; set; }
}