using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace NumberGuessGameApi
{
    public class Program
    {
        //Esta clase se encarga de iniciar la app
            protected Program()
            {
            }
            public static void Main(string[] args)
            {
            //Construye el host de la app con sus configuraciones y la ejecuta

            //Configuración de Serilog
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                 .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                 .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                 .WriteTo.File(
                     path: "logs/game-api.txt",  // UN SOLO ARCHIVO
                     rollingInterval: RollingInterval.Infinite,  // NO crear archivos por día
                     outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                     shared: true  // Permitir escritura concurrente
                 )
                 .CreateLogger();

            try
            {
                Log.Information("=== Iniciando NumberGuessGameApi ===");
                Log.Information("Fecha y hora: {Timestamp}", DateTime.Now);

                // Crear y ejecutar el host de la aplicación
                CreateHostBuilder(args).Build().Run();

                Log.Information("=== Aplicación finalizada correctamente ===");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación falló al iniciar");
                Log.Fatal(ex, "Excepción: {ExceptionType}", ex.GetType().Name);
                Log.Fatal(ex, "Mensaje: {Message}", ex.Message);
            }
            finally
            {
                Log.CloseAndFlush(); // Asegurar que todos los logs se escriban
            }
        }
            public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
                .UseSerilog() // Usar Serilog como sistema de logging
                .ConfigureWebHostDefaults(config => { config.UseStartup<Startup>(); });
    }
}
