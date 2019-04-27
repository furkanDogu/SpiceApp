using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace SpiceApp.Util.DataUtil
{
    public class DBConnection : IDisposable
    {
        private readonly string _connectionString;

        public DBConnection()
        {
            _connectionString = "Data Source=FURKAN;Initial Catalog=OtoKiralama;Integrated Security=True";
        }
        
        private SqlConnection GetSqlConnection()
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            if(conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Open();
            }
            else
            {
                conn.Open();
            }
            return conn;
        }

        public SqlCommand GetSqlCommand()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetSqlConnection();
            return cmd;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
