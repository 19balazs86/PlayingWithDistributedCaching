using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace PlayingWithDistributedCaching.FilterCaching
{
  /// <summary>
  /// Creates a CacheResourceFilter for all occurrence of CacheResourceFilterAttribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class CacheResourceFilterAttribute : Attribute, IFilterFactory
  {
    private readonly DistributedCacheEntryOptions _cacheOptions;

    public bool IsReusable => true; // Reuse the created CacheResourceFilter.

    public CacheResourceFilterAttribute(int absoluteExpiration, int slidingExpiration)
    {
      _cacheOptions = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(absoluteExpiration))
        .SetSlidingExpiration(TimeSpan.FromSeconds(slidingExpiration));
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
      //return serviceProvider.GetService(typeof(CacheResourceFilter)) as IFilterMetadata

      var cache  = serviceProvider.GetService(typeof(IDistributedCache)) as IDistributedCache;
      var logger = serviceProvider.GetService(typeof(ILogger<CacheResourceFilter>)) as ILogger<CacheResourceFilter>;

      return new CacheResourceFilter(cache, _cacheOptions, logger);
    }
  }
}
