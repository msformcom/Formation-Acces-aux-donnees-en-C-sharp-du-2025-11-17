
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.DAL;

namespace Tests
{
    internal class DI
    {
        public static readonly IServiceProvider Services;

        static DI()
        {
            // Création de l'injection de dépendance
            var serviceCollection = new ServiceCollection();

            // Config basée sur fichier xml
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            var config = configBuilder.Build();
            serviceCollection.AddSingleton<IConfiguration>(config);

            serviceCollection.AddTransient<SqlConnection>(_ =>
            {
                var connectionString = config.GetConnectionString("MaSociete");
                // Cette construction sera exécutée à chaque demande de service du type SqlConnection
                return new SqlConnection(connectionString);
            });


            serviceCollection.AddDbContext<MyContext>(optionsBuilder =>
            {
                // optionBuilder => Objet permettant de construire un objet de type DbContextOptions
                // optionsBuilder.UseInMemoryDatabase("Toto");

                optionsBuilder.UseSqlServer("name=MaSocieteCodeFirst");

            });


            // ServiceProvider
            Services = serviceCollection.BuildServiceProvider();
            
        }
    }
}
