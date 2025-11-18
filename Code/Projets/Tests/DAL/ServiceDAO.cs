using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DAL
{
    // But de cette classe => définir la structure d'une donnée extraites 
    // pas tellement la structure d'une table ou les nom des colonnes
    public class ServiceDAO
    {
        // Colonne PK_Service
        public int Id { get; set; }

        // Colonne Libele
        public string Libele { get; set; }

        // Colonne FK_Chef_Service
        public Guid ChefServiceId { get; set; }

        // Virtual => permet dans une classe dérivée de faire un override
        // Le package Microsoft.EntityFrameworkCore.Proxies (Lasy loading)
        // va pouvoir créer une classe dérivée et réécrire la méthode get
        // pour aller chercher les données
        public virtual EmployeDAO ChefService { get; set; }

        // ICollection est IEnumerabme
        // ICollection => add,remove, clear
        public virtual ICollection<EmployeDAO> Employes { get; set; } =new HashSet<EmployeDAO>();

    }
}
