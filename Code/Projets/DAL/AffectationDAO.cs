using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using MaSocieteDAL;

namespace DAL
{
    public class AffectationDAO
    {
        public Guid EmployeId { get; set; }
        public int ServiceId { get; set; }
        public DateTime DateEntree { get; set; } = DateTime.Now;

        public virtual ServiceDAO Service { get; set; }
        public virtual EmployeDAO Employe { get; set; }
    }
}
