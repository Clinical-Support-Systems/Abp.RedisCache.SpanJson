using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Acme.BookStore.Configuration;
using Acme.BookStore.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

namespace Acme.BookStore.Web.Startup
{
    [DependsOn(
        typeof(BookStoreApplicationModule),
        typeof(BookStoreEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpRedisCacheModule))]
    public class BookStoreWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public BookStoreWebModule(IWebHostEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BookStoreWebModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(BookStoreWebModule).Assembly);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(BookStoreConsts.ConnectionStringName);

            Configuration.Navigation.Providers.Add<BookStoreNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(BookStoreApplicationModule).GetAssembly()
                );

            Configuration.Caching.UseRedis(options =>
            {
                // Use the new force Luke.
                options.UseSpanJson();
            });
        }
    }
}