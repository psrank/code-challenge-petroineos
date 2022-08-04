using PetroineosChallenge.Core;
using PetroineosChallenge.Core.Configuration;
using PetroineosChallenge.WorkerService;
using Services;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<ExtractSettings>(hostContext.Configuration);
        services.AddTransient<CronScheduleProvider>();
        services.AddTransient<CsvWriter>();
        services.AddTransient<IPowerService,PowerService>();
        services.AddTransient<ITradeExtractor,TradeExtractor>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();