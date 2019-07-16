using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Binary;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace PlayingWithDistributedCaching
{
  public class Startup
  {
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      var redisConfiguration = _configuration.GetSection("Redis").Get<RedisConfiguration>();

      services.AddSingleton(redisConfiguration); // It is mandatory.

      services.AddStackExchangeRedisExtensions<BinarySerializer>(redisConfiguration);
      //services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      app.UseRouting();

      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}
