using Abp.Configuration.Startup;
using Abp.Dependency;
using System;

namespace Abp.Runtime.Caching.Redis;

/// <summary>
/// AbpRedisCacheOptionsExtensions
/// </summary>
public static class AbpRedisCacheOptionsExtensions
{
    /// <summary>
    /// UseSpanJson
    /// </summary>
    /// <param name="options"></param>
    public static void UseSpanJson(this AbpRedisCacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.AbpStartupConfiguration
            .ReplaceService<IRedisCacheSerializer, SpanJsonRedisCacheSerializer>(DependencyLifeStyle.Transient);
    }
}
