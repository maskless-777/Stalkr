using Neo4j.Driver;
using Stalkr.Models;
using Stalkr.Repositories;
using Microsoft.Extensions.Options;

namespace Stalkr
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AddServices(builder);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            ConfigureApplication(app);
            await app.RunAsync();
        }

        private static void ConfigureApplication(WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<PeopleRepository>();

            builder.Services.AddScoped<IRepository<PeopleModel>, PeopleRepository>();
            builder.Services.AddScoped<IRepository<ClassesModel>, ClassesRepository>();

            // Add controllers
            builder.Services.AddControllers();
        }
    }
}
