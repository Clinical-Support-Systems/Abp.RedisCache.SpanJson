using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Acme.BookStore.EntityFrameworkCore
{
    [DependsOn(
        typeof(BookStoreCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class BookStoreEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BookStoreEntityFrameworkCoreModule).GetAssembly());
        }
    }
}