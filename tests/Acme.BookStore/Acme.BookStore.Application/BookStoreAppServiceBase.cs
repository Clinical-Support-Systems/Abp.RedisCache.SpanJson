using Abp.Application.Services;

namespace Acme.BookStore
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class BookStoreAppServiceBase : ApplicationService
    {
        protected BookStoreAppServiceBase()
        {
            LocalizationSourceName = BookStoreConsts.LocalizationSourceName;
        }
    }
}