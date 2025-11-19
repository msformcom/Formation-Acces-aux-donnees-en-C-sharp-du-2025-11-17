using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DAL
{
    public class BulletinDAO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Montant { get; set; }

        public DateOnly Date { get; set; } = new DateOnly();

        public bool Verse { get; set; } = false;

        public Guid EmployeId { get; set; }

        public virtual EmployeDAO Employe { get; set; }
    }
}
