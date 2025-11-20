
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using MaSocieteDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using MonWebApi.DTO;

var builder = WebApplication.CreateBuilder(args);



#region Mise en place des services

// Builder.Services => ServiceCollection déjà en place dans le WebApplicationBuider
// Avce Configuration et Logging

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<ServiceDAO, ServiceListItemDTO>()
        .ForMember(c=>c.Label,o=>o.MapFrom(c=>c.Libele))
        .ForMember(c=>c.NbEmployes,o=>o.MapFrom(c=>c.Employes.Count()));

    config.CreateMap<ServiceDAO, ServiceDTO>()
     .ForMember(c => c.Label, o => o.MapFrom(c => c.Libele))
     .ForMember(c => c.Employes, o => o.MapFrom(c => c.Employes))
     .ForMember(c => c.NbEmployes, o => o.MapFrom(c => c.Employes.Count()));

    config.CreateMap<Adresse, AdresseDTO>().ReverseMap();

    // Pour la propriété Adresse de la classe EmployeDAO => AutoMapper utilisera le mapping défini audessus
    // Créer un EmployeDTO à partir d'un EmployeDAO
    config.CreateMap<EmployeDAO, EmployeDTO>()
        .ForMember(c => c.Name, o => o.MapFrom(c => c.Nom))
        .ForMember(c => c.Surname, o => o.MapFrom(c => c.Prenom))
        .ForMember(c => c.OutDate, o => o.MapFrom(c => c.DateSortie))
        .ForMember(c => c.EntryDate, o => o.MapFrom(c => c.DateEntree))
        .ForMember(c => c.Adress, o => o.MapFrom(c => c.Adresse))
         .ForMember(c => c.Salaire, o => o.MapFrom(c=>c.Bonus))
         .ForMember(c=>c.Services,o=>o.MapFrom(c=>c.Services))
        // Créer un EmployeDAO à partir d'un EmployeDTO
        .ReverseMap()
        .ForMember(c => c.Bonus, o => o.MapFrom(c => c.Salaire));
       
});

#region OData
var oDataBuilder = new ODataConventionModelBuilder();
// Ajout du controleur Employee
oDataBuilder.EntitySet<EmployeDTO>("Employee");


#endregion



// Permet au middleware de routage des controllers d'instancier les controllers
builder.Services.AddControllers()
    .AddOData(options =>
    {
        // Routes avec /odata/Employee
        options.AddRouteComponents("odata", oDataBuilder.GetEdmModel())
            .Filter()
            .OrderBy()
            .Select()
            .Expand()
            .Count()
            .SetMaxTop(20)
            .EnableQueryFeatures();

    });

// Ajoout du MyContext au services (en tant que scoped)
builder.Services.AddDbContext<MyContext>(optionsBuilder =>
{
    // optionBuilder => Objet permettant de construire un objet de type DbContextOptions
    // optionsBuilder.UseInMemoryDatabase("Toto");

    optionsBuilder.UseSqlServer("name=MaSocieteCodeFirst");
   // optionsBuilder.UseLazyLoadingProxies();

});

// Singleton => une seule instance pour toutes les demandes
builder.Services.AddSingleton<ModelOptions<MyContext>>();


// Ajout de la fenètre de Debug au Logging
builder.Logging.AddDebug();
#endregion


// Construction de l'application
var app = builder.Build();

#region Création de la BDD (si pas déjà fait)

// Il me faut une instance de context => ensureCreated
using (var scope=app.Services.CreateScope())
{
    // Dans le cadre d'une appli web, un scope est créé pour chaque requete
    // Les dépendances peuvent être enregistrée sous 3 formes
    // Singleton => 1 seule instance quel que soit le scope
    // Transient => 1 nouvelle instance à chaque demande indépendamment du scope
    // Scoped => Une instance par scope
    // 1 requete => 1 scope crée => si on demande un MyContext => 1 seul pour toutes les demandes pour cette requete
    // je demande une instance de MyContext
    //app.Services.GetService<MyContext>();//=> new marche pas car il faut un scope
    using (var ctx = scope.ServiceProvider.GetRequiredService<MyContext>())
    {
       
        if (ctx.Database.EnsureCreated())
        {
            // La base vient juste d'être crée
            // Je vais mettre les données initiale

            // Seed => Mettre des données initiales à la création

            // Création de 10 employés
            var employes = Enumerable.Range(1, 99)
                    .Select(c => new EmployeDAO()
                    {
                        Nom = "Nom" + c,
                        Prenom = "Prenom" + c,
                        Code = "Emp" + c,
                        Bonus = c * 1000,
                        DateEntree = new DateOnly(2025, 10, 19)
                    }).ToArray();

            ctx.Employes.AddRange(employes);

            ctx.SaveChanges();


            var services = Enumerable.Range(1, 10)
                    .Select(c => new ServiceDAO()
                    {
                
                        Libele = "Service" + c,
                        ChefServiceId = employes[c].Id
                    }).ToArray();

            ctx.Services.AddRange(services);

            ctx.SaveChanges();

            var r = new Random();
            foreach(var s in services)
            {

                foreach (var e in Enumerable.Range(1, 20).Select(c => r.Next(0, 99)).Distinct().Select(c => employes[c]))
                {
                    s.Employes.Add(e);
                }
            }

        

            ctx.SaveChanges();


        }
    }
    ;

}


    #endregion

    #region Mise en place des middlewares


    // Ajout de Middleware de journalisation
    // La fonction MiddleWare recoit : 
    // 1) le Contexte de la requète à traiter
    // 2) un fonction asynchrone qui appelle le middleware suivant
    app.Use(async (HttpContext context, Func<Task> next) =>
    {

        var dateEntree = DateTime.Now;
        var message = $"Requète entrante vers: {context.Request.Path} at {dateEntree:hh:mm:ss}";
        app.Logger.Log(LogLevel.Information, message);
        // Passage de la requète au middleware suivant
        await next();
        // Après le traietement par les middleware suivants
        var duration = (DateTime.Now - dateEntree).TotalMicroseconds;
        message = $"Requète traitée vers: {context.Request.Path} en {duration} ms";
        app.Logger.Log(LogLevel.Information, message);
    });


//app.Use(async (HttpContext context, Func<Task> next) =>
//{
//    // J'ajoute au Stream de la réponse le message Toto
//    if (context.Request.Path == "/bonjour")
//    {
//        await context.Response.WriteAsync("Bonjour Aussi");
//    }
//    else
//    {
//        await next();
//    }
//});

// Liason de la fonction avec requete Get /Employes
// Le MyContext est fourni par DI => [FromService]
// [FromBody] => Chercher l'objet dans le body de la requete

app.MapGet("/Employes", async (
                HttpContext context, 
                [FromServices] MyContext ctx,
                [FromServices] IMapper mapper
                ) =>
{
    //var employesDTOs = ctx.Employes.Select(c => new EmployeDTO()
    //{
    //    Code = c.Code,
    //    Name = c.Nom,
    //    Surname = c.Prenom,
    //    EntryDate = c.DateEntree,
    //    OutDate = c.DateSortie,
    //    Adress = c.Adresse != null ? new AdresseDTO() { CP = c.Adresse.CP, Ville = c.Adresse.Ville, Ligne = c.Adresse.Ligne } : null
    //}).ToArray();

    var employesDTOs = ctx.Employes.Select(c => mapper.Map<EmployeDTO>(c));


    // EmployeDAO ne doit jamais être renvoyé à l'utilisateur du service
    // Utiliser un DTO => Objet spécialisé
    // DTO => Ne contient pas de connées techniques de la BDD
    // => Ne contient pas toutes les propriétés de navigation ou elles ne sont pas remplies
    // => Données sensible non comprises
    // => Sous ensemble des propriéts du DAO spécifiques au besoin du client 
    await context.Response.WriteAsJsonAsync(employesDTOs);
});

// Route : Forme de l'url qu'on veut gérer
app.MapGet("/Employes/{code}/Services", async (
        HttpContext context,
         [FromServices] MyContext ctx,
         [FromServices] IMapper mapper,
        [FromRoute] string code) =>
{
 
    var employeDAO = ctx.Employes
        .Include(c=>c.ServicesSubordonnes)
        .ThenInclude(c=>c.Employes)
        .First(c => c.Code == code);
    if (employeDAO == null)
    {
        context.Response.StatusCode = 404;
        return;
    }


    //var services = employeDAO.Services.Select(c => mapper.Map<ServiceDTO>(c));
    var services = mapper.Map<IEnumerable<ServiceListItemDTO>>(employeDAO.ServicesSubordonnes);

    await context.Response.WriteAsJsonAsync(services);
});


app.MapControllers();


#endregion


// Démarrage de l'application
app.Run();

