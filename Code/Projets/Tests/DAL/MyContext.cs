using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Tests.DAL
{
    // Représente la partie de la BDD que je vais utiliser avec ce contexxte
    // On va limiter la surface du context
    internal class MyContext : DbContext
    {
        private readonly ModelOptions<MyContext> modelOptions;

        // Pour construire un MyContext il faut préciser les options (chaine de connection, timeout)
        // Ce constructeur les recoit et les passe au constructeur de base
        // Il reçoit également les options de personnalisation de la BDD
        public MyContext(DbContextOptions<MyContext> options, ModelOptions<MyContext> modelOptions) : base(options) 
        {
            this.modelOptions = modelOptions;
            
        }

        // Exécution d'une procédure stockée / vue / query adhoc avec retour des resultats
        public IQueryable<ShortBulletin> GetBulletins(Guid EmployeId)
        {
            return this.Database.SqlQuery<ShortBulletin>($"SELECT montant,date FROM TBL_Bulletins WHERE FK_Employe={EmployeId}");
        }


        public void DeleteBulletin(Guid id)
        {
            this.Database.ExecuteSqlRaw("EXEC DeleteBulletin @id", new SqlParameter("@id", id));
        }

        public DbSet<EmployeDAO> Employes { get; set; }
        public DbSet<ServiceDAO> Services { get; set; }
        public DbSet<BulletinDAO> Bulletins { get; set; }

        // Fonction du contexte permettant de préciser la structuire de la BDD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BulletinDAO>(options =>
            {
                var tableName = this.modelOptions.GetTableName(nameof(BulletinDAO));
                options.ToTable(tableName);

                options.HasKey(c => c.Id).IsClustered(false);
                options.Property(c => c.Id)
                    .HasColumnName(modelOptions.GetPrimaryKeyName(nameof(BulletinDAO)));

                // Optimisation des recherches de bulletin par employé
                options.HasIndex(c => c.EmployeId).IsClustered(true);

                // Pour la compta : index qui est couvrant sur Id,Date,Montant,Verse
                options.HasIndex(c=>c.Date).IsClustered(false).IncludeProperties(c=>new {c.Montant,c.Verse});

                options.Property(c => c.EmployeId).HasColumnName("FK_Employe");

                options.HasOne(c => c.Employe).WithMany(c => c.Bulletins).HasForeignKey(c => c.EmployeId).OnDelete(DeleteBehavior.Cascade);

                // Indication : Utiliser la procédure stockée DeleteBulletion pour supprimer des bulletins
                // => à la place du code SQL normalement généré
                options.DeleteUsingStoredProcedure("DeleteBulletin", options => {
                    
                    options.HasOriginalValueParameter(c=>c.Id);
               });
                // Pour éxécuter un PS en cas de Update d'une entité
                //options.UpdateUsingStoredProcedure("UpdateBulletin", options => {
                //    // Paramètre de la procédure recevant la nouvelle valeur d'un propriété
                //    //options.HasParameter(c => c.Id);
                //    // Paramètre de la procédure recevant l'ancienne valeur d'un propriété
                //    //options.HasOriginalValueParameter(c => c.Id);
                //});

            });

            modelBuilder.Entity<ServiceDAO>(options =>
            {
                var tableName = this.modelOptions.GetTableName(nameof(ServiceDAO));
                options.ToTable(tableName);

                options.HasKey(c => c.Id).IsClustered(false);
                options.Property(c => c.Id)
                    .HasColumnName(modelOptions.GetPrimaryKeyName(nameof(ServiceDAO)))
                    .ValueGeneratedOnAdd();
                options.HasIndex(c => c.Libele).IsUnique();
                options.Property(c => c.Libele).IsUnicode(true).HasMaxLength(100).UseCollation("FRENCH_CI_AI");

                // relation 1 à N entre Services et ChefService
                options.HasOne(c => c.ChefService).WithMany(c => c.ServicesSubordonnes).HasForeignKey(c => c.ChefServiceId)
                            // La suppression d'un chef de service est impossible
                            .OnDelete(DeleteBehavior.Restrict);
                options.Property(c => c.ChefServiceId).HasColumnName("FK_ChefService");

                // Relation de N à N entre Services et les Employe
                options.HasMany(c => c.Employes).WithMany(c => c.Services);

            });
     
            modelBuilder.Entity<EmployeDAO>(options =>
            {
                var tableName = this.modelOptions.GetTableName(nameof(EmployeDAO));
                options.ToTable(tableName);

                options.HasKey(c => c.Id).IsClustered(false);
                options.Property(c => c.Id).HasColumnName(modelOptions.GetPrimaryKeyName(nameof(EmployeDAO)));

                options.Property(c => c.Code).HasMaxLength(5).IsUnicode(false).IsFixedLength(true);

                options.Property(c => c.Nom).HasMaxLength(50).IsRequired().IsUnicode(true);

                options.Property(c => c.Prenom).HasMaxLength(50).IsRequired().IsUnicode(true);

                // ConcurrencyToken => Permet de ne pas écraser cette donnée
                // si une mise à jour concurrente a eu lieu
                options.Property(c => c.Salaire).IsConcurrencyToken(true);

                options.Property(c => c.DateSortie).IsRequired(true);

                options.Property(c => c.DateSortie).IsRequired(false);

                // Optimisation des recherches et des jointures sur ServiceId
                //options.HasIndex(c => c.ServiceId).IsClustered(true);

                // SELECT DateEntree,Id, Nom, Prenom FROM Employes Optimisé car index couvrant
                // SELECT DateEntree,Id, Nom, Prenom, Code  FROM Employes  Pas optimisé
                options.HasIndex(c => c.DateEntree).IncludeProperties(c => new { c.Nom, c.Prenom });

                options.OwnsOne(c => c.Adresse, options => {
                    options.Property(c => c.CP).HasColumnName("CodePostal");
                    options.Property(c => c.Ligne).HasColumnName("Ligne");
                    options.Property(c => c.Ville).HasColumnName("Ville");
                });

            });

            // Seed => Mettre des données initiales à la création

            // Création de 10 employés
            var employes = Enumerable.Range(1, 10)
                    .Select(c => new EmployeDAO()
                    {
                        Nom = "Nom" + c,
                        Prenom = "Prenom" + c,
                        Code = "Emp" + c,
                        Salaire = c * 1000,
                        DateEntree = new DateOnly(2025,10,19)
                    }).ToArray();
        

            var services = Enumerable.Range(1, 4)
                    .Select(c => new ServiceDAO()
                    {
                        Id=c,
                        Libele = "Service" + c,
                        ChefServiceId = employes[c].Id
                    }).ToArray();

            //foreach(var e in employes.Skip(2).Take(4))
            //{
            //    services[0].Employes.Add(e);
            //}

            //foreach (var e in employes.Take(4))
            //{
            //    services[1].Employes.Add(e);
            //}
            //foreach (var e in employes.TakeLast(6))
            //{
            //    services[2].Employes.Add(e);
            //}

            //foreach (var e in employes.Skip(4).Take(3))
            //{
            //    services[3].Employes.Add(e);
            //}


            modelBuilder.Entity<EmployeDAO>().HasData(employes);
            modelBuilder.Entity<ServiceDAO>().HasData(services);

        }



    }
}
