using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SpiceApp.Util.DataUtil
{
    public static class DBAdjuster
    {
        private static DBConnection dbConnection = new DBConnection();
        public static void AdjustReservations()
        {
            dbConnection.OpenConnection();
            // This func. is responsible for checking reservation dates if there is any out dated reservation.
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = DBCommandCreator.EXEC(new string[0], "SP_rezKontrol");
            dbConnection.ExecuteQueries(cmd);
            dbConnection.CloseConnection();

        }
    }
}
