using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using ToDoList;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Tests
{
    public class TestProgram
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        // Скопировано из Program.cs
                        services.AddControllers();
                        services.AddEndpointsApiExplorer();
                        services.AddSwaggerGen();

                        services.AddHttpClient();


                        //// Используем InMemoryDatabase для тестов
                        //services.AddDbContext<ApplicationDbContext>(options =>
                        //    options.UseInMemoryDatabase("TestDatabase")); // Используем InMemory


                        // Настройка CORS 
                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowAllOrigins", policy =>
                            {
                                policy.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                            });
                        });

                        services.AddControllers();
                        services.AddScoped<ToDoService>();
                    });

                    webBuilder.Configure(app =>
                    {

                        app.UseSwagger();
                        app.UseSwaggerUI();

                        app.UseCors("AllowAllOrigins");

                        app.UseHttpsRedirection();

                        app.UseRouting();

                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
        }
    }
}