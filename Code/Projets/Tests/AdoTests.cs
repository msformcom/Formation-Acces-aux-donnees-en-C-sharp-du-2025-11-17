using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

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

  
    public IEnumerable<string> SelectEmployes()
    {
        // Très important : lorsque on utilise des objets qui utilisent des resources du système
        // dans un bloc using
        using (var conn = DI.Services.GetRequiredService<SqlConnection>())
        {
            conn.Open();
            // Nécessité d'écrire la requète
            // Qui peut être dépendante du logiciel de BDD utilisé
            using (var requete = new SqlCommand("SELECT * FROM TBL_Employes", conn))
            {
                using (var reader = requete.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nom = reader.GetString(3);
                        yield return nom;
                    }
                }
                
            }
        }

    }
}
