using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonWebApi.DTO
{
    public class EmployeDTO : IValidatableObject
    {
            [Key]
        public string Code { get; set; }

        [MinLength(2, ErrorMessage ="Le {0} doit avoir au moins {1} caractères")]
        public string Name   { get; set; }
        public string Surname { get; set; }


        // Propriété complexe
        public AdresseDTO? Adress { get; set; }

        public DateOnly EntryDate { get; set; }
        public DateOnly? OutDate { get; set; }
        public ICollection<ServiceDTO> Services { get; set; }

        [Range(0,2000, ErrorMessage ="Le {0} doit être entre {1} et {2}")]
        public Decimal? Salaire { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OutDate is not null && EntryDate > OutDate)
            {
                yield return new ValidationResult($"La {nameof(OutDate)} ne peut être inférieure à {nameof(EntryDate)}");
            }
        }
    }
}
