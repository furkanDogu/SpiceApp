using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using SpiceApp.Util.DateUtil;
using System.Data.SqlClient;
using SpiceApp.DataAccessLayer.Interfaces;


namespace SpiceApp.DataAccessLayer.Concretes
{

    public class CarRepository : IUpdatableRepository<Car>, IDisposable
    {
        private DBConnection dBConnection;
        private int _rowsAffected;

        public CarRepository()
        {
            dBConnection = new DBConnection();
            _rowsAffected = 0;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool DeleteById(int CarID)
        {
            // This function is responsible for deleting a car by carID
            _rowsAffected = 0;

            //open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();

            try
            {
                // create the query
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "aracID" }, "SP_aracPasif");
                DBCommandCreator.AddParameter(cmd, "@aracID", DbType.Int32, ParameterDirection.Input, CarID);

                //execute the query
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing DeleteById() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }

        public bool Insert(Car entity)
        {
            _rowsAffected = 0;
            // this func. helps us adding a new car to a specified company.

            // open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = DBCommandCreator.INSERT(new string[] { "model","ehliyetYasi",
                        "minYas",
                        "anlikKm","airbag",
                        "bagajHacmi","gunlukFiyat",
                        "sirketID","markaID", "gunlukKm" },
                    DBTableNames.Cars);

                // adding parameter values
                DBCommandCreator.AddParameter(cmd, "@model", DbType.String, ParameterDirection.Input, entity.CarModel);
                DBCommandCreator.AddParameter(cmd, "@ehliyetYasi", DbType.Int32, ParameterDirection.Input, entity.RequiredDriverLicenceExp);
                DBCommandCreator.AddParameter(cmd, "@minYas", DbType.Int32, ParameterDirection.Input, entity.RequiredAge);
                DBCommandCreator.AddParameter(cmd, "@airbag", DbType.Boolean, ParameterDirection.Input, entity.HasAirbag);
                DBCommandCreator.AddParameter(cmd, "@bagajHacmi", DbType.String, ParameterDirection.Input, entity.BaggageCapacity);
                DBCommandCreator.AddParameter(cmd, "@gunlukFiyat", DbType.Decimal, ParameterDirection.Input, entity.DailyCost);
                DBCommandCreator.AddParameter(cmd, "@anlikKm", DbType.Int32, ParameterDirection.Input, entity.KmInfo);
                DBCommandCreator.AddParameter(cmd, "@sirketID", DbType.Int32, ParameterDirection.Input, entity.Company.CompanyID);
                DBCommandCreator.AddParameter(cmd, "@markaID", DbType.Int32, ParameterDirection.Input, entity.Brand.BrandID);
                DBCommandCreator.AddParameter(cmd, "@gunlukKm", DbType.Int32, ParameterDirection.Input, entity.DailyKm);


                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }

        public IList<Car> FetchAllByCompany(int CompanyID)
        {
            List<Car> cars = new List<Car>();

            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {

                string condition = "WHERE [sirketID] = @CompanyID";

                // creating a select query with util
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "aracID", "model", "ehliyetYasi", "minYas", "anlikKm", "airbag", "bagajHacmi", "gunlukFiyat", "sirketID", "markaID", "aktiflik" },
                DBTableNames.Cars, condition);

                // adding  companyID parameter to the query.
                DBCommandCreator.AddParameter(cmd, "@CompanyID", DbType.Int32, ParameterDirection.Input, CompanyID);


                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // creating car instances for every row in the response table
                        var entity = new Car
                        {
                            CarID = reader.GetInt32(0),
                            CarModel = reader.GetString(1),
                            RequiredDriverLicenceExp = reader.GetInt32(2),
                            RequiredAge = reader.GetInt32(3),
                            KmInfo = reader.GetInt32(4),
                            HasAirbag = reader.GetBoolean(5),
                            BaggageCapacity = reader.GetString(6),
                            DailyCost = (decimal)reader.GetSqlMoney(7),
                            Company = new CompanyRepository().FetchById(reader.GetInt32(8)),
                            Brand = new BrandRepository().FetchById(reader.GetInt32(9)),
                            isActive = reader.GetBoolean(10)
                        };
                        cars.Add(entity);

                    }
                }
                return cars;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing FetchAll() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }


        }

        public Car FetchById(int CarID)
        {
            // this func. helps us fetching a car by a specified carID
            Car entity = null;

            // open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                //condition to be looked for.
                string condition = "WHERE aracID = @CarID";

                // concating query with the help of DBCommandCreator utility class
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "aracID", "model", "ehliyetYasi", "minYas", "anlikKm", "airbag", "bagajHacmi", "gunlukFiyat", "sirketID", "markaID", "gunlukFiyat", "aktiflik" }, DBTableNames.Cars, condition);
                DBCommandCreator.AddParameter(cmd, "@CarID", DbType.Int32, ParameterDirection.Input, CarID);

                //execute the query
                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // creating car instances for every row in the response table
                        entity = new Car()
                        {
                            CarID = reader.GetInt32(0),
                            CarModel = reader.GetString(1),
                            RequiredDriverLicenceExp = reader.GetInt32(2),
                            RequiredAge = reader.GetInt32(3),
                            KmInfo = reader.GetInt32(4),
                            HasAirbag = (bool)reader.GetBoolean(5),
                            BaggageCapacity = reader.GetString(6),
                            Company = new CompanyRepository().FetchById(reader.GetInt32(8)),
                            Brand = new BrandRepository().FetchById(reader.GetInt32(9)),
                            DailyCost = (decimal)reader.GetSqlMoney(7),
                            isActive = reader.GetBoolean(11)
                        };
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing FetchAll() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public bool ReActivateCarById(int CarID)
        {
            // brings back the deleted car (incase a car is not available for a short period for the reasons like fixing issues so then it can be available and active again).
            _rowsAffected = 0;

            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();

            try
            {
                

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "aracID" }, "SP_aracAktif");
                DBCommandCreator.AddParameter(cmd, "@aracID", DbType.Int32, ParameterDirection.Input, CarID);
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in ReActivateCarById() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }

        }

        public bool Update(Car entity)
        {

            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            _rowsAffected = 0;
            try
            {
                

                cmd.CommandText = "UPDATE tblArac SET ehliyetYasi = @ehliyetYasi, minYas = @minYas, airbag = @airbag, gunlukFiyat = @gunlukFiyat WHERE aracID = @aracID";
                DBCommandCreator.AddParameter(cmd, "@ehliyetYasi", DbType.Int32, ParameterDirection.Input, entity.RequiredAge);
                DBCommandCreator.AddParameter(cmd, "@minYas", DbType.Int32, ParameterDirection.Input, entity.RequiredAge);
                DBCommandCreator.AddParameter(cmd, "@airbag", DbType.Boolean, ParameterDirection.Input, entity.HasAirbag);
                DBCommandCreator.AddParameter(cmd, "@gunlukFiyat", DbType.Decimal, ParameterDirection.Input, entity.DailyCost);
                DBCommandCreator.AddParameter(cmd, "@aracID", DbType.Int32, ParameterDirection.Input, entity.CarID);

                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Update() func. SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }






    }
}
