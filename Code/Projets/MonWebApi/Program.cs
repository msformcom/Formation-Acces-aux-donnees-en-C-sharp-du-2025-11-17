
using AutoMapper;
using MaSocieteDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonWebApi.DTO;

var builder = WebApplication.CreateBuilder(args);

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
        // Créer un EmployeDAO à partir d'un EmployeDTO
        .ReverseMap();
});


// Ajoout du context au services
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

var app = builder.Build();

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

app.Run();

