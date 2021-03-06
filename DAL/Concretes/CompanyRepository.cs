﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data.SqlClient;
using System.Data;


namespace SpiceApp.DataAccessLayer.Concretes
{
    public class CompanyRepository : IDisposable
    {
        private DBConnection dBConnection;
        public CompanyRepository()
        {
            dBConnection = new DBConnection();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public Company FetchById(int CompanyID)
        {
            //responsible for getting company info with given company id.

            Company entity = null;

            // we will execute 2 queries so we need another conn.
            DBConnection tempDB = new DBConnection();
            tempDB.OpenConnection();

            dBConnection.OpenConnection();
           
            SqlCommand cmd = new SqlCommand();
            SqlCommand cmdForScore = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "sirketID", "sirketAd", "tel", "sehir", "adres", "aracSayisi", "sirketPuan" }, DBTableNames.Company, "WHERE sirketID = @CompanyID");
                DBCommandCreator.AddParameter(cmd, "@CompanyID", DbType.Int32, ParameterDirection.Input, CompanyID);

                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // create company instance with fetched info
                        entity = new Company()
                        {
                            CompanyID = reader.GetInt32(0),
                            CompanyName = reader.GetString(1),
                            Phone = reader.GetString(2),
                            City = reader.GetString(3),
                            Address = reader.GetString(4),
                            CarCount = reader.GetInt32(5),
                        };

                        // getting score value from a pre-defined stored procedure

                        cmdForScore.CommandText = DBCommandCreator.EXEC(new string[] { "sirketID" }, "SP_puan");
                        DBCommandCreator.AddParameter(cmdForScore, "@sirketID", DbType.Int32, ParameterDirection.Input, entity.CompanyID);
                        
                        entity.Score = Convert.ToString(tempDB.ExecuteScalar(cmdForScore));

                    }
                }
                return entity;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing FetchByID() in SpiceApp.BusinessLayer.CompanyRepository", ex);

            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
                tempDB.CloseConnection();
            }
        }

    }
}
