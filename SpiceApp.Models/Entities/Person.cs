using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Entities
{
    public class Person
    {
        [Required(ErrorMessage = "PersonID field is required")]
        public int PersonID { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Address field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string Email { get; set; }

        [Required(ErrorMessage = "DriverLicenseDate field is required")]
        public DateTime DriverLicenseDate { get; set; }

        [Required(ErrorMessage = "Birthday field is required")]
        public DateTime Birthday  { get; set; }

        public Company Company { get; set; }
    }
}
