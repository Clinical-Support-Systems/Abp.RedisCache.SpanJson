using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Acme.BookStore
{
    [DependsOn(
        typeof(BookStoreCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class BookStoreApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BookStoreApplicationModule).GetAssembly());
        }
    }
}