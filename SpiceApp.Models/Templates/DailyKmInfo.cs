﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Templates
{
    public class DailyKmInfo
    {
        public int RentID { get; set; }
        public string BrandName { get; set; }
        public string CarModel { get; set; }
        public int DailyKm { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }

    }
}
