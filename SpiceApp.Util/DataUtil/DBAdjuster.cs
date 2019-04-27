using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Util.DataUtil
{
    public static class DBAdjuster
    {
        private static DBConnection dbConnection = new DBConnection();
        public static void AdjustReservations()
        {
            // This func. is responsible for checking  
            using (var cmd = dbConnection.GetSqlCommand())
            {
                cmd.CommandText = DBCommandCreator.EXEC(new string[0], "SP_rezKontrol");
                cmd.ExecuteNonQuery();
            }
        }
    }
}
