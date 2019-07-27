using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PlayingWithDistributedCaching.FilterCaching;
using PlayingWithDistributedCaching.Models;

namespace PlayingWithDistributedCaching.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class DistributedCacheController : ControllerBase
  {
    private readonly Random _random = new Random();

    private const string _keyPrefix = "UserKey_";

    private readonly DistributedCacheEntryOptions _cacheOptions =
      new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
        .SetSlidingExpiration(TimeSpan.FromSeconds(15));

    private readonly IDistributedCache _cache;

    public DistributedCacheController(IDistributedCache cache) => _cache = cache;

    [HttpGet("{id}")]
    public async Task<User> Get(int id)
    {
      byte[] bytes = await _cache.GetAsync($"{_keyPrefix}{id}");

      if (bytes is null || bytes.Length == 0) return null;

      return JsonSerializer.Deserialize<User>(bytes);
    }

    [HttpPost]
    public Task Post([FromBody] User user)
    {
      user.DateCreated = DateTime.Now;

      byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(user);

      return _cache.SetAsync($"{_keyPrefix}{user.Id}", bytes, _cacheOptions);
    }

    [HttpDelete("{id}")]
    public Task Delete(int id)
      => _cache.RemoveAsync($"{_keyPrefix}{id}");

    [HttpGet("cache-resource-filter/{id}")]
    //[ServiceFilter(typeof(CacheResourceFilter<DefaultCacheOptions>))]
    [CacheFilter]
    public async Task<IActionResult> CacheResourceFilter(int id)
    {
      IActionResult[] results = new IActionResult[]
      {
        new OkObjectResult(new User { Id = id, FirstName = "OkObjectResult", DateCreated = DateTime.Now }),
        new JsonResult(new User { Id = id, FirstName = "JsonResult", DateCreated = DateTime.Now }),
        new ContentResult
        {
          Content     = JsonSerializer.Serialize(new User { Id = id, FirstName = "ContentResult", DateCreated = DateTime.Now }),
          ContentType = MediaTypeNames.Application.Json
        },
        new NotFoundObjectResult(new User { Id = id, FirstName = "NotFoundObjectResult", DateCreated = DateTime.Now })
      };

      await Task.Delay(1500);

      return results[_random.Next(0, results.Length)];
    }
  }
}