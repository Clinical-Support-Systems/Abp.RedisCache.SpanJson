using Abp.AspNetCore.Mvc.Views;

namespace Acme.BookStore.Web.Views
{
    public abstract class BookStoreRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected BookStoreRazorPage()
        {
            LocalizationSourceName = BookStoreConsts.LocalizationSourceName;
        }
    }
}
