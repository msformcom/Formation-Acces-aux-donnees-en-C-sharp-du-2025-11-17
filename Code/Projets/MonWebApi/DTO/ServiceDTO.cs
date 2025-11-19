using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using MaSocieteDAL;

namespace MonWebApi.DTO
{
    public class ServiceListItemDTO
    {
        public string Label { get; set; }
        public int NbEmployes { get; set; }
    }
    public class ServiceDTO : ServiceListItemDTO
    {
        public IEnumerable<EmployeDTO>   Employes { get; set; }
    }
}
