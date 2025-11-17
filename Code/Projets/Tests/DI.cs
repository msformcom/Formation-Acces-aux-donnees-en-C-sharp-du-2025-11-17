
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                return new SqlConnection(config.GetConnectionString("MaSociete"));
            });



            // ServiceProvider
            Services = serviceCollection.BuildServiceProvider();
            
        }
    }
}
