using Abp.Modules;
using Abp.Reflection.Extensions;
using Acme.BookStore.Localization;

namespace Acme.BookStore
{
    public class BookStoreCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            BookStoreLocalizationConfigurer.Configure(Configuration.Localization);
            
            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = BookStoreConsts.DefaultPassPhrase;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BookStoreCoreModule).GetAssembly());
        }
    }
}