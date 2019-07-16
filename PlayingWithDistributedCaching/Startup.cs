using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Protobuf;

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

      services.AddStackExchangeRedisExtensions<ProtobufSerializer>(redisConfiguration);
      //services.AddStackExchangeRedisExtensions<MsgPackObjectSerializer>(redisConfiguration);
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
