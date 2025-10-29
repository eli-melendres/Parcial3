using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.OpenApi.Models;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.Migrations.Services;
using Serilog;

namespace NumberGuessGameApi
{
    public class Startup
    {
        //Esta clase se encarga de configurar los servicios y el pipeline de la app
        public IConfiguration Configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Registra los servicios que la app va a usar
        public void ConfigureServices(IServiceCollection services)
        {
            //Configuracion de la db aca?
            services.AddDbContext<GameDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure() // Reintentos automáticos
                )
            );

            //Registrar servicios personalizados (Dependency Injection)
            services.AddScoped<IGameService, GameService>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
       
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Number Guess API", Version = "v1" });
            });
            services.AddMvc().AddMvcOptions(options =>
            {
                //options.Filters.Add<BaseExceptionFilter>();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Registra cada petición HTTP en el archivo .txt
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


            //Migrar db automaticamente
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<GameDbContext>();

                // Aplicar migraciones pendientes automáticamente
                if (env.IsDevelopment())
                {
                    try
                    {
                        context.Database.Migrate();
                        Log.Information("Migraciones aplicadas exitosamente");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error al aplicar migraciones");
                    }
                }
            }
        }
        
    }
}
