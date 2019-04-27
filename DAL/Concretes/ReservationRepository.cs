﻿using System;
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
            //this function will be used for showing reservations either for user or employee. If the given ID belongs to employee, function will show companies reservations.
            List<Reservation> list = new List<Reservation>();
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kullaniciID" }, "SP_rezGoruntule");
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                    CompanyRepository companyRepo = new CompanyRepository();
                    CarRepository carRepo = new CarRepository();
                    UserRepository userRepo = new UserRepository();

                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                var entity = new Reservation()
                                {
                                    Car = carRepo.FetchById(reader.GetInt32(0)),
                                    User = userRepo.FetchById(UserID),
                                    StartingDate = reader.GetDateTime(2),
                                    EndDate = reader.GetDateTime(3),
                                    ReservationID = reader.GetInt32(4),
                                    ReservationMadeAt = reader.GetDateTime(5),
                                };
                                entity.ReservationState = ResvAttrConverter.ConvertResvState(reader.GetBoolean(6), reader.GetBoolean(7), reader.GetBoolean(8));
                                entity.Company = entity.Car.Company;
                                list.Add(entity);
                            }
                        }
                    }

                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllByUserId() func. in SpiceApp.DataAccessLayer.ReservationRepository",ex);
            }
          
        }

        public List<Car> FetchAvailableCarsForResv(int UserID, DateTime startingDate, DateTime endDate)
        { // This function is responsible for fetching all available cars by userID for available reservation

            //first, adjust previous reservations and discard them.
            DBAdjuster.AdjustReservations();
            try
            {
                // will hold valid cars for reservation criterias (startingdate, endtime, user's driver license exp. and user's age)
                List<Car> cars = new List<Car>();

                using (var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.EXEC(new string[] { "kisiID", "basTarih", "bitisTarih" }, "SP_uygunArac");
                    DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, UserID);
                    DBCommandCreator.AddParameter(cmd, "@basTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(startingDate));
                    DBCommandCreator.AddParameter(cmd, "@bitisTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(endDate));

                    // creating repository instances to get brand and company information.
                    CompanyRepository companyRepo = new CompanyRepository();
                    BrandRepository brandRepo = new BrandRepository();

                    using (var reader = cmd.ExecuteReader())
                    {
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
                                    Company = companyRepo.FetchById(reader.GetInt32(9)),
                                    Brand = brandRepo.FetchById(reader.GetInt32(10))
                                };
                                cars.Add(car);
                            }
                        }
                    }

                }
                return cars;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllByUserForResv() in SpiceApp.DataAccessLayer.CarRepository", ex);
            }


        }

        public Reservation FetchById(int id)
        {
            // Todo
            throw new NotImplementedException();
        }

        public bool DeleteById(int ReservationID)
        {
            _rowsAffected = 0;

            using (var cmd = dBConnection.GetSqlCommand())
            {
                cmd.CommandText = DBCommandCreator.EXEC(new string[] { "rezID" }, "SP_rezIptal");
                DBCommandCreator.AddParameter(cmd, "@rezID", DbType.Int32, ParameterDirection.Input, ReservationID);

                _rowsAffected = cmd.ExecuteNonQuery();

                return _rowsAffected > 0;
            }
        }

        public bool Insert(Reservation entity)
        {
            /* Car.CarID, User.UserID, DateTime startingDate, DateTime endDate*/
            _rowsAffected = 0;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.EXEC(new string[] { "aracID", "kullaniciID", "basTarih", "bitisTarih" }, "SP_rezYap");
                    DBCommandCreator.AddParameter(cmd, "@aracID", DbType.Int32, ParameterDirection.Input, entity.Car.CarID);
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, entity.User.UserID);
                    DBCommandCreator.AddParameter(cmd, "@basTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(entity.StartingDate));
                    DBCommandCreator.AddParameter(cmd, "@bitisTarih", DbType.String, ParameterDirection.Input, DateConverter.ToDatabase(entity.EndDate));

                    _rowsAffected = cmd.ExecuteNonQuery();

                    return _rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured while executing MakeReservation() function in SpiceApp.DataAccessLayer.CarRepository", ex);
            }
        }

       

    }
}
