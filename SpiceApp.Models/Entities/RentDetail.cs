using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Entities
{
    public class RentDetail
    {

        public int RentID { get; set; }
        
        public DateTime StartingDate { get; set; }

        public DateTime EndDate { get; set; }

        public int KmUsed { get; set; }

        public decimal Cost { get; set; }
        
        public bool isCarRecievedBack { get; set; }

        /*
            isCarRecivedBack info 
            0 => araba hala müşteride duruyor ve şirkete teslim edilmedi (eğer aracın son teslim tarihi bugün ile aynı yada bugunden küçük ise teslim al butonu gösterilecek)
            1 => araba şirkete geri döndürülmüştür.
        */
        public DateTime RecievedBackAt { get; set; } // eğer tarih boş ise daha teslim alınmamıştır.
        public Car Car { get; set; }
        public User User { get; set; }



    }
}
