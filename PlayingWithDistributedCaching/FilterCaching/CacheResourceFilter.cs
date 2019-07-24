using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace PlayingWithDistributedCaching.FilterCaching
{
  public class CacheResourceFilter : IAsyncResourceFilter
  {
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions;
    private readonly ILogger<CacheResourceFilter> _logger;
    private readonly BinaryFormatter _binaryFormatter;

    public CacheResourceFilter(
      IDistributedCache cache,
      DistributedCacheEntryOptions cacheOptions,
      ILogger<CacheResourceFilter> logger)
    {
      _cache        = cache;
      _cacheOptions = cacheOptions;
      _logger       = logger;

      _binaryFormatter = new BinaryFormatter();

      _logger.LogInformation("CacheResourceFilter is created.");
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
      // Just proceed, if it is not a GET method.
      if (context.HttpContext.Request.Method != HttpMethod.Get.Method)
      {
        await next();
        return;
      }

      string key = context.HttpContext.Request.Path;

      byte[] bytes;

      // --> Get value from the cache.
      try
      {
        bytes = await _cache.GetAsync(key, context.HttpContext.RequestAborted);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to read the cache.");

        // In case of exception just proceed.
        await next();
        return;
      }

      // --> Check value bytes.
      if (bytes is null)
      {
        ResourceExecutedContext executedContext = await next();

        var result = executedContext.Result; // In case of exception it is null.

        var statusCodeActionResult = result as IStatusCodeActionResult;

        // --> Check status code.
        // Return when: Result is null or not an implementation of IStatusCodeActionResult or not OK.
        if (statusCodeActionResult is null ||
           (statusCodeActionResult.StatusCode.HasValue && statusCodeActionResult.StatusCode.Value != 200)) return;

        object cachingObject = result switch
        {
          ObjectResult res  => res.Value,
          ContentResult res => res.Content,
          JsonResult res    => res.Value,
          _                 => null
        };

        if (cachingObject is null) return;

        _logger.LogInformation($"Caching the object for the key: '{key}'.");

        try
        {
          // --> Serialize the object and save in the cache.
          using var memoryStream = new MemoryStream();

          _binaryFormatter.Serialize(memoryStream, cachingObject); // Need Serializable attribute for custom object.

          await _cache.SetAsync(key, memoryStream.ToArray(), _cacheOptions, context.HttpContext.RequestAborted);
        }
        catch (SerializationException ex)
        {
          _logger.LogWarning(ex, "Failed to serialize the {object}.", cachingObject);
        }
        catch (Exception ex)
        {
          _logger.LogWarning(ex, "Failed to set the cache.");
        }
      }
      else
      {
        object deserializedObj;

        using (var memoryStream = new MemoryStream(bytes))
          deserializedObj = _binaryFormatter.Deserialize(memoryStream);

        context.Result = deserializedObj switch
        {
          string content => new ContentResult { Content = content, ContentType = MediaTypeNames.Application.Json },
          _              => new JsonResult(deserializedObj) as IActionResult
        };

        _logger.LogInformation("The value was found in the cache.");
      }
    }
  }
}
