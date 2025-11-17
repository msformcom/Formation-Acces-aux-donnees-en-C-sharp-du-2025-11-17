using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public sealed class AdoTests
{

    // Design pattern Generator / Iterator
    // generator => il génère des éléments à la demande
    IEnumerable<int> Entiers()
    {
        var i = 0;
        while (i<int.MaxValue)
        {
            yield return i;
            i++;
        }
    }


    [TestMethod]
    public void TestEnumerable()
    {
        var liste = SelectEmployes();
        // Itéraotsur Entiers => Take(10)
        foreach(var e in liste.Skip(1).Take(1))
        {
            var a = e;
        }
    }


    [TestMethod]
    public void UpdateEmploye()
    {
        using (var conn = DI.Services.GetRequiredService<SqlConnection>())
        {
            conn.Open();
            // début de transaction
            var transaction = conn.BeginTransaction();
            try
            {
                // Guid pour id employes => générés offline, sécurité, non prévisibilité
                // Attention à l'ordre des opérations pour éviter les DEADLOCKS
                using (var requete1 = new SqlCommand("UPDATE TBL_Employes SET Salaire=Salaire+10 WHERE PK_Employe=@id", conn))
                {   // Pour éviter l'injection sql on donne des paramètre au texte de la requete
                    // => la requète est compilée sur le serveur puis le paramètre est utilisé
                    requete1.Transaction= transaction;
                    requete1.Parameters.AddWithValue("@id", "66a0c229-460e-4a07-8d9e-2384abb93d3d");
                    requete1.ExecuteNonQuery();
                }
                using (var requete2 = new SqlCommand("UPDATE TBL_Employes SET Salaire=Salaire-10 WHERE PK_Employe='4abe54e6-b423-49c1-8c23-3c68c6bf2cc1'", conn))
                {
                    requete2.Transaction = transaction;
                    requete2.ExecuteNonQuery();
                }
                // On arrive ici, on valide
                transaction.Commit();
            }
            catch (Exception)
            {
                // En cas d'erreur j'annule les modification
                transaction.Rollback();
                throw new Exception("Opération annulée");
            }


         
        }
    }

  
    public IEnumerable<string> SelectEmployes()
    {
        // Très important : lorsque on utilise des objets qui utilisent des resources du système
        // dans un bloc using
        using (var conn = DI.Services.GetRequiredService<SqlConnection>())
        {
            conn.Open();
            var transaction=conn.BeginTransaction(IsolationLevel.Snapshot);
            // Read Uncommitted => Lit les données en cours de modification même si non validées
            // Read Committed (Defaut) => Attente de la validation des données lues
            // Snapshot => Lit les données avant les modifications en cours => pas forcément disponible => Peut être non disponible
            
            // Nécessité d'écrire la requète
            // Qui peut être dépendante du logiciel de BDD utilisé
            using (var requete = new SqlCommand("SELECT * FROM TBL_Employes", conn))
            {
                requete.Transaction= transaction;
                using (var reader = requete.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nom = reader.GetString(3);
                        yield return nom;
                    }
                }
                transaction.Commit();
                
            }
        }

    }
}
