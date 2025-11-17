using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tests.DAL
{
    // Représente la partie de la BDD que je vais utiliser avec ce contexxte
    // On va limiter la surface du context
    internal class MyContext : DbContext
    {
        // Pour construire un MyContext il faut préciser les options (chaine de connection, timeout)
        // Ce constructeur les recoit et les passe au constructeur de base
        public MyContext(DbContextOptions<MyContext> options) : base(options) 
        {
            
        }

        public DbSet<EmployeDAO> Employes { get; set; }
        public DbSet<ServiceDAO> Services { get; set; }
    }
}
