using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data.SqlClient;
using SpiceApp.Util.DateUtil;
using SpiceApp.DataAccessLayer.Interfaces;

namespace SpiceApp.DataAccessLayer.Concretes
{
    public class UserRepository : IUpdatableRepository<User>, IDisposable
    {
        private DBConnection dBConnection;

        // will be used to figure out whether the command worked succesfully or not
        private int _rowsAffected;

        public UserRepository()
        {
            dBConnection = new DBConnection();
            _rowsAffected = 0;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public User FetchById(int UserID)
        {
            // responsible for getting user by given user id
            User entity = null;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    // find user with given userID and get "kullaniciAd", "sifre", "kisiID", "rolID", "kullaniciID" fields from db
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "kullaniciAd", "sifre", "kisiID", "rolID", "kullaniciID" }, DBTableNames.User, "WHERE kullaniciID = @kullaniciID");
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // create user instance with the data brought from db
                                entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Person = FetchPersonByID(reader.GetInt32(2)),
                                    Role = new Role() { Name = reader.GetInt32(3) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(3) },
                                    UserID = reader.GetInt32(4)
                                };
                            }
                        }
                    }

                }
                //return user instance
                return entity;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchById() in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }

        public User FetchByUsername(string Username)
        {
            // responsible for getting user from db by given username
            // it will be mainly used in login operation to check if the user exists with given username.

            User entity = null;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand()) // creating command instance
                {
                 
                    // defining commandtext property of command with the help of DBCommandCreator utility class
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] {"kullaniciAd","sifre","kisiID","rolID","kullaniciID" }, DBTableNames.User, "WHERE kullaniciAd = @kullaniciAd");
                    DBCommandCreator.AddParameter(cmd, "@kullaniciAd", DbType.String, ParameterDirection.Input, Username);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                // creating user instance 
                                entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Password = reader.GetString(1),
                                    Person = FetchPersonByID(reader.GetInt32(2)),
                                    Role = new Role() { Name = reader.GetInt32(3) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(3) },
                                    UserID = reader.GetInt32(4)
                                };
                            }
                        }
                    }

                }
                // return  user instance.
                return entity;
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Insert(User entity)
        {
            // responsible for adding new users to the db. Takes a parameter which is type of User.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            try
            {
                // first add the person object to the db. It holds detailed information of user.
                InsertPerson(entity.Person);

                int id=0; // will hold the last added person's id
                using (var cmdID = dBConnection.GetSqlCommand())
                {
                    // the last added person will have the greatest id value so bring it from the db.
                    cmdID.CommandText = DBCommandCreator.SELECT(new string[] { "kisiID" }, DBTableNames.Person, "WHERE kisiID = (SELECT MAX(kisiID) FROM tblKisi)");
                        
                    using(var reader = cmdID.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                id = reader.GetInt32(0);
                            }
                        }
                    }

                }

                using(var cmdUser = dBConnection.GetSqlCommand())
                {
                    // call a stored proc. to add newly created user to the db.
                    cmdUser.CommandText = DBCommandCreator.EXEC(new string[] {"kullaniciAd","sifre","kisiID"}, "SP_kullaniciKayit");
                    DBCommandCreator.AddParameter(cmdUser, "@kullaniciAd", DbType.String, ParameterDirection.Input, entity.Username);
                    DBCommandCreator.AddParameter(cmdUser, "@sifre", DbType.String, ParameterDirection.Input, entity.Password);
                    DBCommandCreator.AddParameter(cmdUser, "@kisiID", DbType.Int32, ParameterDirection.Input, id);
                    _rowsAffected = cmdUser.ExecuteNonQuery();
                }
                // if added, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured while executing Insert() in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }

        public bool Update(User entity)
        {
            // responsible for updating a user's info. Takes a param that is type of User
            // only address, phone and email infos can be changed. So the other given info won't be cared
            // Even if the stated fields didn't change, their original values must be passed to the param of this function.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;   

            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblKisi SET adres = @adres, cepTel = @cepTel, email = @email WHERE kisiID = @kisiID";
                    DBCommandCreator.AddParameter(cmd, "@adres", DbType.String, ParameterDirection.Input, entity.Person.Address);
                    DBCommandCreator.AddParameter(cmd, "@cepTel", DbType.String, ParameterDirection.Input, entity.Person.Phone);
                    DBCommandCreator.AddParameter(cmd, "@email", DbType.String, ParameterDirection.Input, entity.Person.Email);
                    DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, entity.Person.PersonID);
                    _rowsAffected = cmd.ExecuteNonQuery();
                }

                // if updated, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Update() func. in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }

        public List<User> FetchAllCustomers()
        {
            // responsible for fetching all customers from the db.
            // it is planned to be used in employee's screen while making reservation.
            // Firstly, employee should choose a customer to make reservation. This func. can be used right there. 

            List<User> list = new List<User>();
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "kullaniciAd", "kisiID", "rolID", "kullaniciID" }, DBTableNames.User, "WHERE rolID = 1000");
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                // create user instance with the data brought from the db
                                var entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Role = new Role() { Name = reader.GetInt32(2) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(2) },
                                    UserID = reader.GetInt32(3),
                                    Person = FetchPersonByID(reader.GetInt32(1))
                                };
                                // add user instance to the list
                                list.Add(entity);
                            }
                        }
                    }

                }
                // return the list which is full of customers
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllCustomers() in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
            
        }

        public bool ChangePassword (int UserID, string NewPassword)
        {
            _rowsAffected = 0;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblKullanici SET sifre = @sifre WHERE kullaniciID = @kullaniciID";
                    DBCommandCreator.AddParameter(cmd, "@sifre",DbType.String, ParameterDirection.Input, NewPassword);
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                    _rowsAffected = cmd.ExecuteNonQuery();

                }
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in ChangePassword() in ", ex);
            }
        }
        

        //USER DETAILS
        private Person FetchPersonByID(int PersonID)
        {
            // responsible for bringing person with given id  from the db.

            Person entity = null;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "ad", "soyad", "adres", "cepTel", "email", "ehliyetVerilisTarihi", "dogumTarih", "sirketID", "kisiID" },DBTableNames.Person,
                        "WHERE kisiID = @kisiID");
                    DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, PersonID);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                // create person instance 
                                entity = new Person()
                                {
                                    Name = reader.GetString(0),
                                    Surname = reader.GetString(1),
                                    Address = reader.GetString(2),
                                    Phone = reader.GetString(3),
                                    Email = reader.GetString(4),
                                    DriverLicenseDate = reader.GetDateTime(5),
                                    Birthday = reader.GetDateTime(6),
                                    Company = new CompanyRepository().FetchById(reader.GetInt32(7)),
                                    PersonID =  reader.GetInt32(8)
                                };
                            }
                        }
                    }
                }
                // return newly fetched person instance
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchPersonByID function, SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }
        private bool InsertPerson(Person entity)
        {
            _rowsAffected = 0;
            // responsible for adding new person to the db
            try
            {
                using (var cmdPerson = dBConnection.GetSqlCommand())
                {

                    cmdPerson.CommandText = DBCommandCreator.EXEC(new string[] { "ad", "soyad", "adres", "cepTel", "email", "ehliyet", "dogTarih" }, "SP_kisiKayit");
                    
                    // give input parameters' values to the query.
                    DBCommandCreator.AddParameter(cmdPerson, "@ad", DbType.String, ParameterDirection.Input, entity.Name);
                    DBCommandCreator.AddParameter(cmdPerson, "@soyad", DbType.String, ParameterDirection.Input, entity.Surname);
                    DBCommandCreator.AddParameter(cmdPerson, "@email", DbType.String, ParameterDirection.Input, entity.Email);
                    DBCommandCreator.AddParameter(cmdPerson, "@adres", DbType.String, ParameterDirection.Input, entity.Address);
                    DBCommandCreator.AddParameter(cmdPerson, "@cepTel", DbType.String, ParameterDirection.Input, entity.Phone);
                    DBCommandCreator.AddParameter(cmdPerson, "@ehliyet", DbType.Date, ParameterDirection.Input, DateConverter.ToDatabase(entity.DriverLicenseDate));
                    DBCommandCreator.AddParameter(cmdPerson, "@dogTarih", DbType.Date, ParameterDirection.Input, DateConverter.ToDatabase(entity.Birthday));
                    // execute the stored procedure.
                    _rowsAffected = cmdPerson.ExecuteNonQuery();
                }
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in InsertPerson function, SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }
    }
}
