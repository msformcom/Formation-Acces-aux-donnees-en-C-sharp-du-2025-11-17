
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MaSocieteDAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MonWebApi.DTO;

namespace MonWebApi.ODataControllers
{
    [Route("odata/Employee")]
   
    public class EmployeeController : ODataController
    {
        private readonly MyContext ctx;
        private readonly IMapper mapper;

        public EmployeeController(MyContext ctx, IMapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        [EnableQuery]
        [Route("{code}")]
        public SingleResult<EmployeDTO> Employee([FromRoute]string code)
        {
            // requete renvoyant 1 seul résultat (normalement)
            var query= ctx.Employes.Where(c=>c.Code==code).ProjectTo<EmployeDTO>(mapper.ConfigurationProvider);
            // => on prend le premier
            return SingleResult.Create(query);
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<EmployeDTO> Employes()
        {
            return ctx.Employes.ProjectTo<EmployeDTO>(mapper.ConfigurationProvider);
        }
    }
}
