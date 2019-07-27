using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace PlayingWithDistributedCaching.FilterCaching
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  [DebuggerDisplay("CacheFilterAttribute: CacheOptionProvider={CacheOptionProvider}")]
  public class CacheFilterAttribute : Attribute, IFilterFactory
  {
    private readonly Type _cacheFilterType = typeof(CacheResourceFilter<>);

    public bool IsReusable => true;

    public Type CacheOptionProvider { get; private set; }

    public CacheFilterAttribute(Type cacheOptionProvider = null)
      => CacheOptionProvider = cacheOptionProvider ?? typeof(DefaultCacheOptions);

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
      if (serviceProvider is null)
        throw new ArgumentNullException(nameof(serviceProvider));

      Type[] typeArguments = { CacheOptionProvider };
      Type cacheFilterType = _cacheFilterType.MakeGenericType(typeArguments);
      //object o           = Activator.CreateInstance(cacheFilterType);

      return serviceProvider.GetRequiredService(cacheFilterType) as IFilterMetadata;
    }
  }
}
