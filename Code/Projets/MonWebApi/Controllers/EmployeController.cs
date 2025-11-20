using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MaSocieteDAL;
using Microsoft.AspNetCore.Mvc;
using MonWebApi.DTO;

namespace MonWebApi.Controllers
{
    // Classe de controller Api => 
    // Hérite de Controller
    // Attribut [ApiController]
    [ApiController]
    // Pour atteindre les méthode de ce controller l'url doit commencer par /Employe
    [Route("Employe")]
    public class EmployeController : Controller
    {
        private readonly MyContext ctx;



        // Je demande le contexte de données à l'injection

        public EmployeController(MyContext ctx)
        {
            this.ctx = ctx;
        }


        // GET : /Employe/Bonjour
        // GET : /Employe/Hello
        [HttpGet("Bonjour/{nom}")]
        [HttpGet("Hello/{nom}")]
        public string Bonjour(string nom = "")
        {
            return $"Bonjour {nom}";
        }

        [HttpGet("")]

        // GET : /Employe?minSalaire=1000
        public IEnumerable<EmployeDTO> GetEmployes(
            [FromServices] IMapper mapper,
            [FromQuery] Decimal? minSalaire
            ){
            return ctx.Employes
                .Where(c=>minSalaire==null|| c.Bonus>=minSalaire)
                // Créer un DTO par mappage mais en limitant les propriétés utilisées dans le DAO
                .ProjectTo<EmployeDTO>(mapper.ConfigurationProvider);
                    
        }

        [HttpPost("")]
        // POST /Employe
        public ActionResult<EmployeDTO> InsertEmploye(
            //[FromQuery] decimal Salaire ,
            [FromBody]EmployeDTO employe, IServiceProvider provider)
        {
            // Validation du DTO entrant (voir Attributs et IValidatableObject sur le DTO)
            if (!ModelState.IsValid)
            {
                provider.GetRequiredService<ILogger<EmployeController>>()
                    .Log(   LogLevel.Error,
                            ModelState.Values.Where(c=>c.Errors.Count>0).First().Errors.First().ErrorMessage);
                return BadRequest(new Exception(ModelState.Values.Where(c => c.Errors.Count > 0).First().Errors.First().ErrorMessage));
            }

            // mapper demandé à l'injecteur de dépendance au moment / si nécessaire
            var mapper=provider.GetService<IMapper>();
            var employeDAO = mapper.Map<EmployeDAO>(employe);
          

            ctx.Employes.Add(employeDAO);
         
            
            ctx.SaveChanges();
            return mapper.Map<EmployeDTO>(employeDAO);
        }


        [HttpPut("")]
        // POST /Employe
        public ActionResult<EmployeDTO> UpdateEmploye(
            //[FromQuery] decimal Salaire ,
            [FromBody] EmployeDTO employe, IServiceProvider provider)
        {
            // Validation du DTO entrant (voir Attributs et IValidatableObject sur le DTO)
            if (!ModelState.IsValid)
            {
                provider.GetRequiredService<ILogger<EmployeController>>()
                    .Log(LogLevel.Error,
                            ModelState.Values.Where(c => c.Errors.Count > 0).First().Errors.First().ErrorMessage);
                return BadRequest(new Exception(ModelState.Values.Where(c => c.Errors.Count > 0).First().Errors.First().ErrorMessage));
            }

            // mapper demandé à l'injecteur de dépendance au moment / si nécessaire
            var mapper = provider.GetService<IMapper>();

            // je recherche le DAO à modifier dans la BDD
            var employeDAO = ctx.Employes.FirstOrDefault(c=>c.Code==employe.Code);

            if(employeDAO is null)
            {
                return NotFound();
            }

            // Injectionn des données du DTO dans le DAO trouvé
            mapper.Map(employe, employeDAO);

            // ici, je marque le bonus comme non modifié
            // l'update ne comprendra pas sa valeur
            ctx.Entry(employeDAO).Property(c => c.Bonus).IsModified = false;

            ctx.SaveChanges();
            return mapper.Map<EmployeDTO>(employeDAO);
        }
    }
}
