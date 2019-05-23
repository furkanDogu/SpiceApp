using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data;
using SpiceApp.Util.DateUtil;
using SpiceApp.Util.AttributeUtil;
using SpiceApp.DataAccessLayer.Interfaces;
using System.Data.SqlClient;

namespace SpiceApp.DataAccessLayer.Concretes
{
    public class ReservationRepository : IRepository<Reservation>, IDisposable
    {
        private DBConnection dBConnection;
        private int _rowsAffected;

        public ReservationRepository()
        {
            dBConnection = new DBConnection();
            _rowsAffected = 0;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<Reservation> FetchAllByUserId(int UserID)
        {
            // check for out dated reservations, if there is then cancel them.
            DBAdjuster.AdjustReservations();

            //this function will be used for showing reservations either for user or employee. If the given ID belongs to employee, function will show companies reservations.
            List<Reservation> list = new List<Reservation>();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                dBConnection.OpenConnection();
                // defining the command text and giving input parameter's value

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_rezGoruntule");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);
                using (reader = dBConnection.DataReader(cmd))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // create reservation instance
                            var entity = new Reservation()
                            {
                                Car = new CarRepository().FetchById(reader.GetInt32(1)),
                                User = new UserRepository().FetchById(UserID),
                                StartingDate = reader.GetDateTime(3),
                                EndDate = reader.GetDateTime(4),
                                ReservationID = reader.GetInt32(0),
                                ReservationMadeAt = reader.GetDateTime(5),
                            };
                            // To define reservation state, we have 3 different boolean values in database.
                            // So ConvertResvState function composes these 3 values to one to make client's work easier.
                            entity.ReservationState = ResvAttrConverter.ConvertResvState(reader.GetBoolean(6), reader.GetBoolean(7), reader.GetBoolean(8));
                            entity.Company = entity.Car.Company;
                            list.Add(entity);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllByUserId() func. in SpiceApp.DataAccessLayer.ReservationRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }

        }

        public List<Car> FetchAvailableCarsForResv(int UserID, DateTime startingDate, DateTime endDate)
        { // This function is responsible for fetching all available cars by userID for available reservation

            // will hold valid cars for reservation criterias (startingdate, endtime, user's driver license exp. and user's age)
            List<Car> cars = new List<Car>();

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();

            //first, adjust previous reservations and discard them.
            DBAdjuster.AdjustReservations();
            try
            {

                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kisiID", "basTarih", "bitisTarih" }, "SP_uygunArac");
                DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, UserID);
                DBCommandCreator.AddParameter(cmd, "@basTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(startingDate));
                DBCommandCreator.AddParameter(cmd, "@bitisTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(endDate));

                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // creating car instances for every row in the response table
                        var car = new Car()
                        {
                            CarID = reader.GetInt32(0),
                            CarModel = reader.GetString(1),
                            RequiredDriverLicenceExp = reader.GetInt32(2),
                            RequiredAge = reader.GetInt32(3),
                            KmInfo = reader.GetInt32(5),
                            HasAirbag = (bool)reader.GetBoolean(6),
                            BaggageCapacity = reader.GetString(7),
                            DailyCost = (decimal)reader.GetSqlMoney(8),
                            Company = new CompanyRepository().FetchById(reader.GetInt32(9)),
                            Brand = new BrandRepository().FetchById(reader.GetInt32(10))
                        };
                        cars.Add(car);
                    }
                }


                return cars;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllByUserForResv() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }


        }

        public Reservation FetchById(int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int ReservationID)
        {
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();

            // responsible for cancelling reservation with the given res. id.
            _rowsAffected = 0;

            try
            {
                // create the query              
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "rezID" }, "SP_rezIptal");
                DBCommandCreator.AddParameter(cmd, "@rezID", DbType.Int32, ParameterDirection.Input, ReservationID);

                //execute the command
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in DeleteById() func. in SpiceApp.DataAccessLayer.ReservationRepository", ex);
            }
            finally
            {
                // close connection
                dBConnection.CloseConnection();
            }








        }

        public bool Insert(Reservation entity)
        {
            /* Car.CarID, User.UserID, DateTime startingDate, DateTime endDate  fields need to be given in the parameter */
            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();

            _rowsAffected = 0;
            try
            {
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "aracID", "kullaniciID", "basTarih", "bitisTarih" }, "SP_rezYap");
                DBCommandCreator.AddParameter(cmd, "@aracID", DbType.Int32, ParameterDirection.Input, entity.Car.CarID);
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, entity.User.UserID);
                DBCommandCreator.AddParameter(cmd, "@basTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(entity.StartingDate));
                DBCommandCreator.AddParameter(cmd, "@bitisTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(entity.EndDate));

                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                return _rowsAffected > 0;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing MakeReservation() function in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }



    }
}
