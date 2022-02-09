namespace JARS_API.Dtos
{
    public class CategoryWalletDto
    {
        public int WalletId { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int CurrentCategoryLevel { get; set; }
    }
}
