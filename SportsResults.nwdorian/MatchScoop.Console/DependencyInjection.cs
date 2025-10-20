using MatchScoop.Console.Email;
using MatchScoop.Console.Jobs;
using MatchScoop.Console.Options;
using MatchScoop.Console.Results;
using Microsoft.Extensions.DependencyInjection;

namespace MatchScoop.Console;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.ConfigureOptions();
        services.ConfigureFeatures();
        services.ConfigureJobs();

        return services;
    }

    private static void ConfigureOptions(this IServiceCollection services)
    {
        services.AddOptions<EmailOptions>().BindConfiguration(nameof(EmailOptions));
        services.AddOptions<ScrapingOptions>().BindConfiguration(nameof(ScrapingOptions));
    }

    private static void ConfigureFeatures(this IServiceCollection services)
    {
        services.AddScoped<SendResultsEmail>();
        services.AddScoped<GetResults>();
    }

    private static void ConfigureJobs(this IServiceCollection services)
        => services.AddHostedService<Worker>();

}
