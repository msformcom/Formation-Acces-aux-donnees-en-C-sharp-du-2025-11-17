using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DAL
{
    [Table("TBLEmployes")]
    public  class EmployeDAO
    {

        [Column("PKEmploye")]
        public Guid Id { get; set; } = Guid.NewGuid(); // PAs new Guid() car 000000-0000

        [MaxLength(5)]
     
        public string Code { get; set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }


        // Propriété complexe
        public Adresse? Adresse { get; set; }

        public DateOnly DateEntree { get; set; }
        public DateOnly? DateSortie { get; set; }

        public Decimal Salaire { get; set; }



        // Propriété de navigation Services
        // Relation Employe-Service N à N
        public virtual ICollection<ServiceDAO> Services { get; set; } = new HashSet<ServiceDAO>();

        // Un employé peut être le chef de plusieurs Service (1 à N)
        public virtual ICollection<ServiceDAO> ServicesSubordonnes { get; set; } = new HashSet<ServiceDAO>();

        public virtual ICollection<BulletinDAO> Bulletins { get; set; } = new HashSet<BulletinDAO>();
    }
}
