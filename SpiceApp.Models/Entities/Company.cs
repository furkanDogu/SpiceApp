using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpiceApp.Models.Entities
{
    public class Company
    {
        [Required(ErrorMessage = "Phone field is required")]
        [StringLength(20, MinimumLength = 3)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "CompanyID field is required")]
        public int CompanyID { get; set; }

        public string Phone { get; set; }


        public string City { get; set; }

        public string Address { get; set; }

        public int CarCount { get; set; }


        public string Score { get; set; }
       
    }
}
