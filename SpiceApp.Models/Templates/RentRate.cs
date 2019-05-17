using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Templates
{
    public class RentRate
    {
        public int CarID { get; set; }
        public string  BrandName { get; set; }
        public string CarModel { get; set; }
        public decimal MonthRate { get; set; }
        public string Term { get; set; }
    }
}
