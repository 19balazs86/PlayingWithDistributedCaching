using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

      services.AddStackExchangeRedisCache(options =>
      {
        options.Configuration = "localhost";
        options.InstanceName  = "KeyPrefix";
      });

      initStackExchangeRedisExtensions(services);

      // It is not needed. The new filter is creating on the fly in the attribute.
      // Not using the ServiceFilterAttribute.
      //services.AddSingleton<CacheResourceFilter>();

      // TODO: Remove this workaround from .NET Core 3 preview 7.
      services.Add(new ServiceDescriptor(
        typeof(IActionResultExecutor<JsonResult>),
        Type.GetType("Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor, Microsoft.AspNetCore.Mvc.Core"),
        ServiceLifetime.Singleton));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();

      app.UseRouting();

      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private void initStackExchangeRedisExtensions(IServiceCollection services)
    {
      var redisConfiguration = _configuration.GetSection("Redis").Get<RedisConfiguration>();

      services.AddSingleton(redisConfiguration); // It is mandatory.

      services.AddStackExchangeRedisExtensions<BinarySerializer>(redisConfiguration);
      //services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
    }
  }
}
