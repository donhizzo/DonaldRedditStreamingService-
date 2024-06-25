// See https://aka.ms/new-console-template for more information
using ExternalServices;
using Reddit.AuthTokenRetriever;
using Reddit;
using Microsoft.Extensions.DependencyInjection;
using ExternalServices.DataMemory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Reddit.Controllers;
using System;
using System.Timers;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello World!, THis is a Reddit Streaming app that makes a call to reddit API every minute to check for new post, highest upvotes and the highest post by a user");


var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddSingleton<IMemory, InMemory>()
            .AddSingleton<IReditAPIService, ReditAPIService>()
            .BuildServiceProvider();

string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())    
    .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);

IConfiguration config = builder.Build();

string ClientId = config["AppSettings:ClientId"];
string ClientSecret = config["AppSettings:ClientSecret"];
string SubredditName = config["AppSettings:SubredditName"];

Console.WriteLine("Authentication started");
var secrets = serviceProvider.GetService<IReditAPIService>().Authenticate(true,ClientId,ClientSecret).Result;
Console.WriteLine("Authentication completed");
Console.WriteLine(" Post stats will be start displaying in about 1 minute ");
var _timer = new System.Timers.Timer(1 * 60 * 1000); // 2 minutes in milliseconds

_timer.AutoReset = true;
_timer.Start();
_timer.Enabled = true;
while (_timer.Enabled)
{
    _timer.Elapsed += (sender, e) => StartProcess(sender, e);
}
Console.ReadLine();
Console.WriteLine("Press the Enter key to exit the program.");



void StartProcess(object sender, ElapsedEventArgs e)
{
    
    serviceProvider.GetService<IReditAPIService>().Run(ClientId,ClientSecret,SubredditName,secrets.refresh_token,secrets.access_token);
}


