using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DAL
{
    // But de cette classe => définir la structure d'une donnée extraites 
    // pas tellement la structure d'une table ou les nom des colonnes
    internal class ServiceDAO
    {
        // Colonne PK_Service
        public int Id { get; set; }

        // Colonne Libele
        public string Libele { get; set; }

        // Colonne FK_Chef_Service
        public Guid ChefServiceId { get; set; }


        //public EmployeDAO ChefService { get; set; }

        // ICollection est IEnumerabme
        // ICollection => add,remove, clear
        //public ICollection<EmployeDAO> Employes { get; set; }

    }
}
