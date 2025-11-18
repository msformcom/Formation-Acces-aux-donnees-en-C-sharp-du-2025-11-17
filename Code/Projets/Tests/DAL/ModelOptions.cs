using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tests.DAL
{
    // Classe d'option de Model Créating pour un contexte
    internal class ModelOptions<T> where T:DbContext
    {
        // Objectif => GetTableName("EmployeDAO") => "TBL_Employe"
        public  Func<string, string> GetTableName { get; set; } = (string s) => "TBL_"+s.Replace("DAO","")+"s";
        public Func<string, string> GetPrimaryKeyName { get; set; } = (string s) => "PK_" + s.Replace("DAO", "");
        public Func<string, string> GetForeignKeyName { get; set; } = (string s) => "FK_" + s.Replace("DAO", "");
    }
}
