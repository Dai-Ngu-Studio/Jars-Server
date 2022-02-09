using JARS_API.Dtos;
using JARS_DAL.Models;

namespace JARS_API
{
    public static class Extension
    {
        public static WalletDto AsDto(this Wallet wallet)
        {
            return new WalletDto
            {
                Name = wallet.Name,
                Percentage = wallet.Percentage.Value,
                StartDate = wallet.StartDate.Value,
                WalletAmount = wallet.WalletAmount.Value,
                AccountId = wallet.AccountId,
            };
        }
        public static CategoryWalletDto AsCateWalletDto(this CategoryWallet categoryWallet)
        {
            return new CategoryWalletDto
            {
                Name = categoryWallet.Name,
                WalletId = categoryWallet.WalletId.Value,
                CurrentCategoryLevel = categoryWallet.CurrentCategoryLevel.Value,
                ParentCategoryId = categoryWallet.ParentCategoryId.Value,
            };
        }
    }
}
