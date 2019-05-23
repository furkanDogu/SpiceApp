using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace SpiceApp.Models.Entities
{
    public class Car : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    
        public int CarID { get; set; }

        public string CarModel { get; set; }
        
        public int RequiredDriverLicenceExp { get; set; }
        
        public int RequiredAge { get; set; }
        
        public int KmInfo { get; set; }
        
        public bool HasAirbag { get; set; }
        
        public string BaggageCapacity { get; set; }
        
        public decimal DailyCost { get; set; }

        public bool isActive { get; set; }
        public Brand Brand { get; set; }

        public Company Company { get; set; }

        public int DailyKm { get; set; }











    }
}
