using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DAL
{
    internal class EmployeDAO
    {
        public Guid Id { get; set; }

        public int MyProperty { get; set; }
        public string Code { get; set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }

        public DateOnly DateEntree { get; set; }
        public DateOnly DateSortie { get; set; }

        public Decimal Salaire { get; set; }

        public int ServiceId { get; set; }

        // Propriété de navigation
        //public ServiceDAO Service { get; set; }
    }
}
