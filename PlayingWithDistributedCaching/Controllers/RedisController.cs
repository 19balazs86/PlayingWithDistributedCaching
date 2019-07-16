using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayingWithDistributedCaching.Models;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PlayingWithDistributedCaching.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class RedisController : ControllerBase
  {
    private readonly IRedisCacheClient _redis;

    private IRedisDatabase _redisDb => _redis.Db0;

    private const string _keyPrefix = "UserKey_";

    public RedisController(IRedisCacheClient redis) => _redis = redis;

    [HttpGet]
    public async Task<IEnumerable<User>> Get()
    {
      IEnumerable<string> keys = await _redisDb.SearchKeysAsync($"{_keyPrefix}*");

      IDictionary<string, User> users = await _redisDb.GetAllAsync<User>(keys);

      return users.Select(x => x.Value);
    }

    [HttpGet("{id}")]
    public Task<User> Get(int id)
      => _redisDb.GetAsync<User>($"{_keyPrefix}{id}");

    [HttpPost]
    public Task<bool> Post([FromBody] User user)
    {
      user.DateCreated = DateTime.Now;

      return _redisDb.AddAsync($"{_keyPrefix}{user.Id}", user, TimeSpan.FromSeconds(15), When.NotExists);
    }

    [HttpPut]
    public Task<bool> Put([FromBody] User user)
    {
      user.DateModified = DateTime.Now;

      return _redisDb.ReplaceAsync($"{_keyPrefix}{user.Id}", user, TimeSpan.FromSeconds(15), When.Exists);
    }

    [HttpDelete("{id}")]
    public Task<bool> Delete(int id)
      => _redisDb.RemoveAsync($"{_keyPrefix}{id}");
  }
}
