
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            // Transient => Une nouvelle instance par demance
            serviceCollection.AddTransient<SqlConnection>(_ =>
            {
                var connectionString = config.GetConnectionString("MaSociete");
                // Cette construction sera exécutée à chaque demande de service du type SqlConnection
                return new SqlConnection(connectionString);
            });

            // Configuration du logging
            serviceCollection.AddLogging(options =>
            {
                // Ajour de la fenètre de debbug aux sorties de logging
                options.AddDebug();
            });

            serviceCollection.AddDbContext<MyContext>(optionsBuilder =>
            {
                // optionBuilder => Objet permettant de construire un objet de type DbContextOptions
                // optionsBuilder.UseInMemoryDatabase("Toto");

                optionsBuilder.UseSqlServer("name=MaSocieteCodeFirst");
                //optionsBuilder.UseLazyLoadingProxies();

            });

            // Singleton => une seule instance pour toutes les demandes
            serviceCollection.AddSingleton<ModelOptions<MyContext>>();

            serviceCollection.AddTransient<HttpClient>(s =>
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri( s.GetService<IConfiguration>().GetSection("Services:MaSociete").Value);
                return httpClient;
            });

            // ServiceProvider
            Services = serviceCollection.BuildServiceProvider();
            
        }
    }
}
