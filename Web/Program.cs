using Serilog;

namespace Web;

file static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt",
                rollingInterval: RollingInterval.Hour,
                rollOnFileSizeLimit: true)
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            await using var app = builder.Build();

            app.UseSerilogRequestLogging();

            await app.RunAsync();
        }
        catch (HostAbortedException)
        {
        }
        catch (Exception e)
        {
            Log.Fatal($"Application not started. {e}");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}