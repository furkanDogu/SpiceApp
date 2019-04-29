using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace SpiceApp.Util.DataUtil
{
    public class DBConnection
    {

        string ConnectionString = "Data Source = FURKAN; Initial Catalog = OtoKiralama; Integrated Security = True";
        private SqlConnection con;

        public void OpenConnection()
        {
            con = new SqlConnection(ConnectionString);
            con.Open();
        }


        public void CloseConnection()
        {
            con.Close();
        }


        public int ExecuteQueries(SqlCommand cmd)
        {
            cmd.Connection = con;
            return  cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            cmd.Connection = con;
            return cmd.ExecuteScalar();
        }


        public SqlDataReader DataReader(SqlCommand cmd)
        {
            cmd.Connection = con;
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
    }
}
