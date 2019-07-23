# Playing with distributed caching

#### Resources

- Personally, I prefer the [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-3.0) (Memory, SQL, [Redis](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-3.0#distributed-redis-cache)).
- [StackExchange.Redis.Extensions](https://github.com/imperugo/StackExchange.Redis.Extensions) - This library extends the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).
- Michael Coding Spot: [Cache Implementations](https://michaelscodingspot.com/cache-implementations-in-csharp-net/) (In-Memory).
- Using `Resource Filters` for caching
  - Code Maze: Good explanation about [the basics of Filters in ASP.NET](https://code-maze.com/filters-in-asp-net-core-mvc/).
  - Microsoft Docs: [Filters in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.0).

##### Setup a Redis server on Windows.

1. Download the redis server (zip version) from [MicrosoftArchive/redis/releases](https://github.com/MicrosoftArchive/redis/releases)
2. Run the server: redis-server.exe
3. Run the client (optional): redis-cli.exe | [Redis commands](https://redis.io/commands)

Install it from: [Chocolatey Galery](https://chocolatey.org/packages/redis-64).
