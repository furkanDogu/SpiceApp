using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data.Common;
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
            Company entity = null;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    string condition = "WHERE sirketID = @CompanyID";
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] {"sirketID","sirketAd","tel","sehir","adres","aracSayisi","sirketPuan"}, DBTableNames.Company, condition);
                    DBCommandCreator.AddParameter(cmd, "@CompanyID", DbType.Int32, ParameterDirection.Input, CompanyID);

                    using(var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while(reader.Read())
                            {
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
                                using(var cmdForScore = dBConnection.GetSqlCommand())
                                {
                                    cmdForScore.CommandText = DBCommandCreator.EXEC(new string[] { "sirketID" }, "SP_puan");
                                    DBCommandCreator.AddParameter(cmdForScore, "@sirketID", DbType.Int32, ParameterDirection.Input, entity.CompanyID);

                                    var result = cmdForScore.ExecuteScalar();
                                    entity.Score = Convert.ToString(result);
                                }
                            }
                        }
                    }
                    
                }

                return entity;

            }catch(Exception ex)
            {
                throw new Exception("An error occured while executing FetchByID() in SpiceApp.BusinessLayer.CompanyRepository", ex);

            }
        }

    }
}
