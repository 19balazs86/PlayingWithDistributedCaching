using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PlayingWithDistributedCaching.Models;

namespace PlayingWithDistributedCaching.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class DistributedCacheController : ControllerBase
  {
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

      return JsonSerializer.Parse<User>(bytes);
    }

    [HttpPost]
    public Task Post([FromBody] User user)
    {
      user.DateCreated = DateTime.Now;

      byte[] bytes = JsonSerializer.ToUtf8Bytes(user);

      return _cache.SetAsync($"{_keyPrefix}{user.Id}", bytes, _cacheOptions);
    }

    [HttpDelete("{id}")]
    public Task Delete(int id)
      => _cache.RemoveAsync($"{_keyPrefix}{id}");
  }
}