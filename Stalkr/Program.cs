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
            builder.Services.Configure<Neo4jsettings>(
                builder.Configuration.GetSection("Neo4j")
            );

            builder.Services.AddSingleton<IDriver>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>().GetSection("Neo4j");
                string uri = config["Uri"];
                string user = config["User"];
                string password = config["Password"];
                return GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            });

            builder.Services.AddScoped<IRepository<MajorModel>, MajorRepository>();
            builder.Services.AddScoped<IRepository<ClassesModel>, ClassesRepository>();
            builder.Services.AddScoped<IRepository<PeopleModel>, PeopleRepository>();
            builder.Services.AddScoped<IRepository<HobbyModel>, HobbyRepository>();
			builder.Services.AddScoped<IRepository<SchoolsModel>, SchoolRepository>();
            builder.Services.AddScoped<KnowsRepository>();
            builder.Services.AddScoped<EnrolledInRepository>();
            builder.Services.AddScoped<EnjoysRepository>();
            builder.Services.AddScoped<AttendsRepository>();
            builder.Services.AddScoped<PrereqRepository>();

            builder.Services.AddControllers();
        }

    }
}
