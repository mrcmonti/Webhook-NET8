using AirlineSendAgent.App;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IAppHost, AppHost>();
        services.AddDbContext<SendAgentDbContext>(optionsAction: options =>
        {
            options.UseSqlServer("Server=localhost;Database=AirlineSendAgent;User Id=sa;Password=Pa55w0rd!;Trust Server Certificate=True;");
        });

        services.AddHttpClient();
        services.AddSingleton<IWebhookClient, WebhookClient>();
    })
    .UseServiceProviderFactory(new DefaultServiceProviderFactory())
    .Build();

host.Services.GetService<IAppHost>()?.Run();