using System;
using Microsoft.Extensions.Caching.Distributed;

namespace PlayingWithDistributedCaching.FilterCaching
{
  public interface ICacheOptionProvider
  {
    DistributedCacheEntryOptions Value { get; }
  }

  public class DefaultCacheOptions : ICacheOptionProvider
  {
    public DistributedCacheEntryOptions Value { get; } =
      new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(20))
        .SetSlidingExpiration(TimeSpan.FromSeconds(5));
  }
}
