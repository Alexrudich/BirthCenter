using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BirthCenter.DataLoader.Services;

namespace BirthCenter.DataLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var generator = host.Services.GetRequiredService<PatientGenerator>();
            var apiClient = host.Services.GetRequiredService<ApiClient>();

            logger.LogInformation("Starting to generate 100 patients...");

            var patients = generator.Generate(100);
            logger.LogInformation("Generated {Count} patients. Starting upload...", patients.Count);

            var successCount = 0;
            foreach (var patient in patients)
            {
                var success = await apiClient.CreatePatientAsync(patient);
                if (success) successCount++;

                // Небольшая задержка, чтобы не нагружать API
                await Task.Delay(50);
            }

            logger.LogInformation("Completed! Successfully uploaded {SuccessCount} of {TotalCount} patients",
                successCount, patients.Count);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Читаем URL из конфигурации
                    var apiUrl = context.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:7000";

                    services.AddSingleton<PatientGenerator>();
                    services.AddHttpClient<ApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    });

                    services.AddLogging(configure => configure.AddConsole());
                });
    }
}