using Microsoft.Extensions.DependencyInjection;
using MaSocieteDAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Builder.Services => ServiceCollection déjà en place dans le WebApplicationBuider
// Avce Configuration et Logging


// Ajoout du context au services
builder.Services.AddDbContext<MyContext>(optionsBuilder =>
{
    // optionBuilder => Objet permettant de construire un objet de type DbContextOptions
    // optionsBuilder.UseInMemoryDatabase("Toto");

    optionsBuilder.UseSqlServer("name=MaSocieteCodeFirst");
    //optionsBuilder.UseLazyLoadingProxies();

});

// Singleton => une seule instance pour toutes les demandes
builder.Services.AddSingleton<ModelOptions<MyContext>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();



app.Run();

