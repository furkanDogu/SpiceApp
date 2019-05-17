using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Templates;
using SpiceApp.Util.DataUtil;
using System.Data;
using SpiceApp.DataAccessLayer.Interfaces;
using System.Data.SqlClient;
using SpiceApp.Util.DateUtil;

namespace SpiceApp.DataAccessLayer.Concretes
{
    public class ReportRepository : IDisposable
    {
        private DBConnection dBConnection;
        public ReportRepository()
        {
            dBConnection = new DBConnection();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public List<DailyKmInfo> DailyKmReport(int userID)
        {
            List<DailyKmInfo> data = new List<DailyKmInfo>();
            SqlDataReader reader = null;
            try
            {
                dBConnection.OpenConnection();
                SqlCommand cmd = new SqlCommand();
                

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_gunlukKmListele");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, userID);

                reader = dBConnection.DataReader(cmd);
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var temp = new DailyKmInfo()
                        {
                            RentID = reader.GetInt32(0),
                            BrandName = reader.GetString(1),
                            CarModel = reader.GetString(2),
                            DailyKm = reader.GetInt32(3),
                            Date = reader.GetDateTime(4),
                            State = "Bilgi Yok"
                        };
                        data.Add(temp);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in DailyKmReport() func. in SpiceApp.DataAccessLayer.ReportRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }

        }

        public List<DailyKmInfo> DailyKmReportByRentID(int UserID, int RentID)
        {
            List<DailyKmInfo> data = new List<DailyKmInfo>();
            SqlDataReader reader = null;
            try
            {
                dBConnection.OpenConnection();
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID", "kiraID" }, "SP_gunlukYapilanKmGoruntule");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);
                DBCommandCreator.AddParameter(cmd, "@kiraID", DbType.Int32, ParameterDirection.Input, RentID);

                reader = dBConnection.DataReader(cmd);
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var temp = new DailyKmInfo()
                        {
                            RentID = reader.GetInt32(0),
                            BrandName = reader.GetString(1),
                            CarModel = reader.GetString(2),
                            DailyKm = reader.GetInt32(3),
                            Date = reader.GetDateTime(4),
                            State = reader.GetBoolean(5) ? "Aşıldı" : "Aşılmadı"
                        };
                        data.Add(temp);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in DailyKmReportByRentID() func. in SpiceApp.DataAccessLayer.ReportRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }
    
        public List<RentRate> MonthlyRentRate(int UserID, DateTime Term)
        {
            List<RentRate> data = new List<RentRate>();
            SqlDataReader reader = null;
            try
            {
                dBConnection.OpenConnection();
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = DBCommandCreator.EXEC(new string[] {"kullaniciID", "term" },"SP_aylikAracOran");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);
                DBCommandCreator.AddParameter(cmd, "@term", DbType.DateTime, ParameterDirection.Input, DateConverter.ToDatabase(Term));

                reader = dBConnection.DataReader(cmd);
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var temp = new RentRate()
                        {
                            CarID = reader.GetInt32(0),
                            BrandName = reader.GetString(1),
                            CarModel = reader.GetString(2),
                            MonthRate = reader.GetDecimal(3),
                            Term = reader.GetString(4)
                        };
                        data.Add(temp);
                    }
                }
                return data;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in MonthlyRentRate() func. in SpiceApp.DataAccessLayer.ReportRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public List<OverKmInfo> OverKmInfo(int UserID)
        {
            SqlDataReader reader = null;
            List<OverKmInfo> data = new List<OverKmInfo>();
            try
            {
                dBConnection.OpenConnection();
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_gunlukAsimOran ");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                reader = dBConnection.DataReader(cmd);
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var temp = new OverKmInfo()
                        {
                            CompanyName = reader.GetString(0),
                            CompanyBalance = reader.GetDecimal(1),
                            Score = reader.GetDecimal(2),
                            OverKmRate = reader.GetDecimal(3),
                            Term = reader.GetString(4)
                        };
                        data.Add(temp);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in OverKmInfo() func. in SpiceApp.DataAccessLayer.ReportRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public List<CompanyBalanceInfo> CompanyBalanceInfo(int UserID)
        {
            List<CompanyBalanceInfo> data = new List<CompanyBalanceInfo>();
            SqlDataReader reader = null;
            try
            {
                dBConnection.OpenConnection();
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_sirketDurum");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                reader = dBConnection.DataReader(cmd);
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        var temp = new CompanyBalanceInfo()
                        {
                            CarCount = reader.GetInt32(0),
                            TotalIncome = reader.GetDecimal(1),
                            TotalExpenses = reader.GetDecimal(2),
                            NetValue = reader.GetDecimal(3)
                        };
                        data.Add(temp);
                    }
                }
                return data;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in CompanyBalanceInfo() func. in SpiceApp.DataAccessLayer.ReportRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }


    }
}
