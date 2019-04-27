using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpiceApp.Models.Entities
{
    public class Brand
    {
        public int BrandID { get; set; }

        [Required(ErrorMessage = "BrandName field is required")]
        [StringLength(50, MinimumLength = 3)]
        public string BrandName { get; set; }
    }
}
