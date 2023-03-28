using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Abp.RedisCache.SpanJson.Tests
{
    public abstract class TestBaseWithLocalIocManager : IDisposable
    {
        protected IIocManager LocalIocManager;

        protected TestBaseWithLocalIocManager(ITestOutputHelper output)
        {
            LocalIocManager = new IocManager();
            Output = output;
        }

        public ITestOutputHelper Output { get; }

        public virtual void Dispose()
        {
            LocalIocManager.Dispose();
        }
    }
}
