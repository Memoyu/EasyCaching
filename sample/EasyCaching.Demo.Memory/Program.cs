// See https://aka.ms/new-console-template for more information
using EasyCaching.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

Console.WriteLine("Start Cache Test");

IServiceCollection services = new ServiceCollection();

services.AddMemoryCache();
services.AddEasyCaching(option =>
{
    option.UseInMemory("m1")
    .WithJson("json");
});

var provider = services.BuildServiceProvider();
var ec = provider.GetRequiredService<IEasyCachingProvider>();
var mc = provider.GetRequiredService<IMemoryCache>();

var count = 10000;
var sw = new Stopwatch();
sw.Start();
for (int i = 0; i < count; i++)
{
    var key = "ec-memory-key-" + i ;
    ec.Set(key, i, TimeSpan.FromMinutes(10));
}
sw.Stop();
Console.WriteLine($"ec-Set  count:{count},time:{sw.Elapsed.TotalMilliseconds}ms");

sw.Restart();
for (int i = 0; i < count; i++)
{
    var key = "mc-memory-key-" + i;
    mc.Set(key, i, TimeSpan.FromMinutes(10));
}
sw.Stop();
Console.WriteLine($"mc-Set  count:{count},time:{sw.Elapsed.TotalMilliseconds}ms");


Console.WriteLine("End");