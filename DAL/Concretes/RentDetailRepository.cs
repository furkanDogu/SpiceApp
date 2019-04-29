using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data;
using SpiceApp.Util.AttributeUtil;
using SpiceApp.DataAccessLayer.Interfaces;
using System.Data.SqlClient;

namespace SpiceApp.DataAccessLayer.Concretes
{
    public class RentDetailRepository : IDisposable, IRepository<RentDetail>
    {
        private DBConnection dBConnection;
        private int _rowsAffected;
        public RentDetailRepository()
        {
            dBConnection = new DBConnection();
            _rowsAffected = 0;
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public RentDetail FetchById(int id)
        {
            // responsible for getting a rent detail with given id from the db.

            //open connection
            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();
            SqlDataReader reader = null;

            try
            {
                RentDetail entity = null;

                // will select all the fields in rent detail table.
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "*" }, DBTableNames.RentDetail, "WHERE kiraID = @kiraID");
                DBCommandCreator.AddParameter(cmd, "@kiraID", DbType.Int32, ParameterDirection.Input, id);

                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // creating rent detail instance from fetched db values.
                        entity = new RentDetail()
                        {
                            StartingDate = reader.GetDateTime(2),
                            EndDate = reader.GetDateTime(3),
                            KmUsed = reader.GetInt32(4),
                            RentID = reader.GetInt32(5),
                            Cost = reader.GetInt32(6),
                            isCarRecievedBack = reader.GetBoolean(7),
                            RecievedBackAt = reader.IsDBNull(8) ? new DateTime(1111, 11, 11) : reader.GetDateTime(8)
                        };
                        // we have ids of corresponding car and user so get them by using their repositories
                        entity.Car = new CarRepository().FetchById(reader.GetInt32(0));
                        entity.User = new UserRepository().FetchById(reader.GetInt32(1));

                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchById() function in SpiceApp.DataAccessLayer.RentDetailRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public bool Insert(RentDetail entity)
        {
            // responsible for adding new rent detail to the db.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            //open connection
            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();

            try
            {
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "rezID" }, "SP_anahtarTeslim");
                DBCommandCreator.AddParameter(cmd, "@rezID", DbType.Int32, ParameterDirection.Input, entity.RentID);

                // execute the query
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                // if added, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in ReservaationRepository in SpiceApp.DataAccessLayer.ReservationRepository ", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }

        public List<RentDetail> FetchAllRentDetail(int UserID)
        {
            // check for out dated reservations, if there is then cancel them
            DBAdjuster.AdjustReservations();

            //responsible for fetching all rent details from db according to given userID.
            // if userID belongs to a customer, stored procedure will return customer's rent details
            // if userID belongs to a employee, stored procedure will return employee's company's rent details

            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                List<RentDetail> rents = new List<RentDetail>();

                // define command text with the help of DBCommandCreator utility class then give user ıd parameter to the query.
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_kiraGoruntule");
                DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // create rent detail instance
                        var entity = new RentDetail()
                        {
                            StartingDate = reader.GetDateTime(2),
                            EndDate = reader.GetDateTime(3),
                            KmUsed = reader.GetInt32(4),
                            Cost = reader.GetInt32(5),
                            RentID = reader.GetInt32(6),
                            isCarRecievedBack = reader.GetBoolean(7),
                            RecievedBackAt = reader.IsDBNull(8) ? new DateTime(1111, 11, 11) : reader.GetDateTime(8)
                        };
                        entity.Car = new CarRepository().FetchById(reader.GetInt32(0));
                        entity.User = new UserRepository().FetchById(reader.GetInt32(1));

                        // add the instance to the list
                        rents.Add(entity);
                    }
                }
                return rents;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllRentDetail() func. in SpiceApp.DataAccessLayer.RentDetailRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public bool ReturnCarToCompany(int RentID, int KmInfo, int Score)
        {

            // responsible for completing the whole rent process. When the customer return the car to company, rent process ends.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kiraID", "anlikKm", "puan" }, "SP_aracIade");
                DBCommandCreator.AddParameter(cmd, "@kiraID", DbType.Int32, ParameterDirection.Input, RentID);
                DBCommandCreator.AddParameter(cmd, "@anlikKm", DbType.Int32, ParameterDirection.Input, KmInfo);
                DBCommandCreator.AddParameter(cmd, "@puan", DbType.Int32, ParameterDirection.Input, Score);
                _rowsAffected = dBConnection.ExecuteQueries(cmd);
            
                // if succesful, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in ReturnCarToCompany() in SpiceApp.DataAccessLayer.RentDetailRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }



    }
}
