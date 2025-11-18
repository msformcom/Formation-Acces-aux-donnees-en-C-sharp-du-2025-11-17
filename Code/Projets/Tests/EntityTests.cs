using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.DAL;

namespace Tests;

[TestClass]
public class EntityTests
{

    [TestMethod]
    public void LectureProprieteDeNavigation()
    {
        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            //ctx.Database.EnsureDeleted();
            //ctx.Database.EnsureCreated();


            // Chargement Eager => En Avance => Include
            // Si je sais à l'avance que vais utiliser la données dans la propriété de navigation
            var service1 = ctx.Services.Include(c=>c.ChefService).First();
            var chefService1 = service1.ChefService;

            // Chargement explicit => Je charge la propriété de navigation au moment et si où j'en ai besoin
            var service2 = ctx.Services.Skip(1).First();

            // De manière explicite je charge le Chef de Service
            // But => Eviter de charger des données de manière systématiques si le besoin est ponctuel
            // Pour une collection => Collection
            ctx.Entry(service2).Reference(c => c.ChefService).Load();
            var chefService2 = service2.ChefService;

            // Lazy Loading => Package supplémentaire pour activer le chargement des propriétés de navigation
            // lorsque ont les lit
            var service3 = ctx.Services.Skip(2).First();
            var chefService3 = service3.ChefService;


        }
    }

    [TestMethod]
    public void ChangeTracker()
    {
        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            var requete = ctx.Employes.Where(c => c.Salaire < 10000000M);
            var trackedEntities = ctx.ChangeTracker.Entries().ToList();
            foreach (var entity in requete)
            {
                entity.Salaire += 1;
                trackedEntities = ctx.ChangeTracker.Entries().ToList();
            }

            
            trackedEntities = ctx.ChangeTracker.Entries().ToList();
            try
            {
                ctx.SaveChanges();
                trackedEntities = ctx.ChangeTracker.Entries().ToList();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Gestion d'une mise à jour concurrente
                throw;
            }
            catch (DbUpdateException ex)
            {

            }


        }
    }

    [TestMethod]
    public void SelectEmployes()
    {
        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            // Log d'un message (necéssité d'avoir un logger dans l'injection )
            DI.Services.GetService<ILogger<EntityTests>>()?.Log(LogLevel.Warning,"Début de SelectEmployes");


            var l = new List<EmployeDAO>().Where(c => c.Salaire > 1000);
        
            var lMaterialisee = l.ToList();
            lMaterialisee.Add(new EmployeDAO());
           
            // Cet objet est une requète => Select * FROM TBL_Employes
             var tousLesEmployes= ctx.Employes;
            // Cet objet est une requète => Select * FROM TBL_Employes WHERE Salaire>1000
            
            var employesGrosSalaire = tousLesEmployes.Where(c => c.Salaire > 1000);

            // employesGrosSalaire est IEnumerable 



            // SELECT * FROM TBL_Employes WHERE CP='24000' ORDER BY Nom,Prenom
            var requete2 = tousLesEmployes.Where(c=>c.Adresse.CP!="24000").OrderBy(c=>c.Nom).ThenBy(c=>c.Prenom)
                .AsEnumerable()
                // A partir 
                .Where(c => 
                Math.Cosh((Double)c.Salaire) > 1
                ).Take(1);


            var resultatRequete2 = requete2.ToList();



            // Sur Enumeration => La requete envoyée
            var listeMaterialisee=employesGrosSalaire.ToList();
            foreach(var e in employesGrosSalaire)
            {

            }

            // SELECT Count(*) FROM TBL_Employes WHERE Salaire>1000 
            var c = employesGrosSalaire.Count();
            var s = employesGrosSalaire.Sum(c => c.Salaire);

        }
    }




    [TestMethod]
    public void InsertEmployes()
    {

        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();



            var employe1 = new EmployeDAO()
            {
                Code = "AZ456",
                Nom = "Gates",
                Prenom = "Bill",
                DateEntree = new DateOnly(2025, 10, 1),
                DateSortie = new DateOnly(2025, 10, 2),
                Salaire = 1000M
        
            };
            var employe2 = new EmployeDAO()
            {
                Code = "AE456",
                Nom = "MMauras",
                Prenom = "Dominique",
                DateEntree = new DateOnly(2025, 10, 1),
                DateSortie = new DateOnly(2025, 10, 2),
                Salaire = 1001M
            };
            ctx.Employes.AddRange(employe1, employe2);
            ctx.SaveChanges();
        }
    }

    [TestMethod]
    public void InsertServices()
    {
        using (var ctx = DI.Services.GetRequiredService<MyContext>())
        {
            ctx.Database.EnsureDeleted();
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

        //using (var ctx = DI.Services.GetRequiredService<MyContext>())
        //{
        //    Assert.AreEqual(ctx.Services.Count(), 1, "Le service n'a pas été inséré");
        //}

    }
}
