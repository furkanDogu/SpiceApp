using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Entities
{
    public class Role
    {
        [Required(ErrorMessage = "RoleID field is required")]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        [StringLength(20, MinimumLength = 5)]
        public string Name { get; set; }
    }
}
