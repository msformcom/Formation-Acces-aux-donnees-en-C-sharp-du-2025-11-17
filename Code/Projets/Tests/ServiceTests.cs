using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MonWebApi.DTO;

namespace Tests;

[TestClass]
public class ServiceTests
{
    [TestMethod]
    public async Task Bonjour()
    {
        using (var httpClient = DI.Services.GetRequiredService<HttpClient>())
        {
            var requete = await httpClient.GetAsync("/Employe/Bonjour/Dom");
            var resultat = await  requete.Content.ReadAsStringAsync();
            Assert.AreEqual(resultat, "Bonjour Dom");
        }
    }

    [TestMethod]
    public async Task Employe()
    {
        using (var httpClient = DI.Services.GetRequiredService<HttpClient>())
        {
            var requete = await httpClient.GetAsync("/Employe");
            var resultat = await requete.Content.ReadFromJsonAsync<IEnumerable<EmployeDTO>>();
            Assert.IsTrue(resultat.Count() ==99);
        }
    }

    [TestMethod]
    public async Task EmployesOData()
    {
        using (var httpClient = DI.Services.GetRequiredService<HttpClient>())
        {
            // var requete2 = oDataClient.For(x.Employes).Filter(c => c.Salaire > 0).Select(c => new { c.Name, c.Surname }).OrderBy(c => c.Salaire);
            // => odata/Employee?$filter=Salaire gt 0&$select=Name,Surname&$orderby=Salaire

            var requete = await httpClient.GetAsync("/odata/Employee?$top=10");
            var resultat = await requete.Content.ReadFromJsonAsync<ODataResponse<IEnumerable<EmployeDTO>>>();
            Assert.IsTrue(resultat.Value.Count() == 10);
        }
    }


    [TestMethod]
    public async Task PostEmploye()
    {
        using (var httpClient = DI.Services.GetRequiredService<HttpClient>())
        {
            var employe = new EmployeDTO()
            {
                Name = "Dominique",
                Code = "AK47",
                Surname = "Mauras",
                Salaire = 1999M,
                EntryDate = DateOnly.FromDateTime(DateTime.Now),
                OutDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1))
            };
            try
            {
                var reponse = await httpClient.PutAsJsonAsync("/Employe", employe);
                if (reponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var erreur = await  reponse.Content.ReadAsStringAsync();
                    throw new Exception("Validation incorrecte");
                }
                var employeInsere = await reponse.Content.ReadFromJsonAsync<EmployeDTO>();
                Assert.AreEqual(employe.Name, employeInsere.Name);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }

}
