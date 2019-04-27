using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Util.DataUtil
{
    public static class DBTableConverter
    {
        public static Dictionary<DBTableNames, string> Tables = new Dictionary<DBTableNames, string>
        {
            {DBTableNames.Brand, "tblMarka"},
            {DBTableNames.Cars, "tblArac"},
            {DBTableNames.RentDetail, "tblKiraDetay"},
            {DBTableNames.Company, "tblSirket"},
            {DBTableNames.Role, "tblRol"},
            {DBTableNames.User, "tblKullanici"},
            {DBTableNames.Person, "tblKisi"},
            {DBTableNames.ReservationDetail,"tblRezervDetay"}
        };
        
    }
}
