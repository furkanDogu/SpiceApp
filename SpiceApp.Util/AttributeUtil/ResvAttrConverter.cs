using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Util.AttributeUtil
{
    public static class ResvAttrConverter
    {
        public static int ConvertResvState(bool ReservationState, bool DeliveryToCustomer, bool inCompany)
        {
            /*
             Result Value Meanings 
             0 => İptal edilmiş
             1 => İptal edilebilir (yanında iptal et butonu gösterilecek iki rol içinde) (Eğer bugün rezervasyonun başlama tarihiyle aynı ise "Teslim et" butonu göster çalışanda) 
             2 => Araç hala müşteride duruyor. (Müşteri rolündeki kullanıcıda "Müşteride" yaz, Çalışan rolündeki kullanıcıya "İade al" butonu göster")
             3 => Müşteri şirkete arabayı geri teslim etmiş, kiralama işlemi tamamlanmış.
             */
            int result = 0;
            if (!ReservationState && !DeliveryToCustomer && inCompany)
                result = 0;
            else if (ReservationState && !DeliveryToCustomer && inCompany)
                result = 1;
            else if (!ReservationState && DeliveryToCustomer && !inCompany)
                result = 2;
            else if (!ReservationState && DeliveryToCustomer && inCompany)
                result = 3;
            return result;
        }
    }
}
