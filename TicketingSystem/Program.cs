using Funzo.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicketingSystem.Persistence;
using TicketingSystem.UseCases;

namespace TicketingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    AddConverters(options.JsonSerializerOptions);
                });

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                AddConverters(options.SerializerOptions);
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("App");
            });

            // Tickets
            AddUseCases(builder.Services);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static JsonSerializerOptions AddConverters(JsonSerializerOptions options)
        {
            options.Converters.Add(new UnionConverterFactory());
            options.Converters.Add(new ResultConverterFactory());
            options.Converters.Add(new OptionConverterFactory());

            return options;
        }

        private static IServiceCollection AddUseCases(IServiceCollection services)
        {
            var useCases = typeof(IUseCase<,>).Assembly.GetTypes().Where(t => t.GetInterface(typeof(IUseCase<,>).Name) is not null).ToArray();

            foreach (var useCase in useCases)
            {
                services.AddScoped(useCase);
            }

            return services;
        }
    }
}
