using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Entities
{
    public class Reservation
    {

        public int ReservationID { get; set; }

        public Car Car { get; set; }
        public User User { get; set; }
        public Company Company { get; set; }

        public DateTime StartingDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ReservationMadeAt { get; set; }

        public int ReservationState { get; set; }

    }
}
