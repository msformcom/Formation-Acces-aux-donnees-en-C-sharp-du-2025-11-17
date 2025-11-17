using Microsoft.Extensions.DependencyInjection;
using Tests.DAL;

namespace Tests;

[TestClass]
public class EntityTests
{
    [TestMethod]
    public void InsertServices()
    {
        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            //ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            var newService = new ServiceDAO()
            {
             
                Libele = "Directtion"
            };

            // Services => liste de Services en mémoire => La BDD n'est pas sollicitée
        
            ctx.Services.Add(newService);
            // ChangeTracker => 1 insertion 

            // met à jour la BDD => Envoi de SQL si BDD relationnelle
            ctx.SaveChanges();


        }

        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            Assert.AreEqual(ctx.Services.Count(), 1,"Le service n'a pas été inséré");
        }

     }
}
