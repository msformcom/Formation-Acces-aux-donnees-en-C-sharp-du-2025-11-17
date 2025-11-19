using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonWebApi.DTO
{
    public class EmployeDTO
    {
      
        public string Code { get; set; }

        public string Name   { get; set; }
        public string Surname { get; set; }


        // Propriété complexe
        public AdresseDTO? Adress { get; set; }

        public DateOnly EntryDate { get; set; }
        public DateOnly? OutDate { get; set; }

 


    }
}
