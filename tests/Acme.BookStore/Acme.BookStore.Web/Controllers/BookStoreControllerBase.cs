using Abp.AspNetCore.Mvc.Controllers;

namespace Acme.BookStore.Web.Controllers
{
    public abstract class BookStoreControllerBase: AbpController
    {
        protected BookStoreControllerBase()
        {
            LocalizationSourceName = BookStoreConsts.LocalizationSourceName;
        }
    }
}