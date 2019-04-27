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
            User entity = null;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {

                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "kullaniciAd", "sifre", "kisiID", "rolID", "kullaniciID" }, DBTableNames.User, "WHERE kullaniciID = @kullaniciID");
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, UserID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Person = FetchPersonByID(reader.GetInt32(2)),
                                    Role = new Role() { Name = reader.GetInt32(3) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(3) }
                                };
                            }
                        }
                    }

                }
                return entity;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public User FetchByUsername(string Username)
        {
            User entity = null;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                 
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] {"kullaniciAd","sifre","kisiID","rolID","kullaniciID" }, DBTableNames.User, "WHERE kullaniciAd = @kullaniciAd");
                    DBCommandCreator.AddParameter(cmd, "@kullaniciAd", DbType.String, ParameterDirection.Input, Username);

                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Password = reader.GetString(1),
                                    Person = FetchPersonByID(reader.GetInt32(2)),
                                    Role = new Role() { Name = reader.GetInt32(3) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(3) }
                                };
                            }
                        }
                    }

                }
                return entity;
                
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Insert(User entity)
        {
            _rowsAffected = 0;
            try
            {
                InsertPerson(entity.Person);
                int id=0; // will hold the last added person's id
                using (var cmdID = dBConnection.GetSqlCommand())
                {
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
                    cmdUser.CommandText = DBCommandCreator.EXEC(new string[] {"kullaniciAd","sifre","kisiID"}, "SP_kullaniciKayit");
                    DBCommandCreator.AddParameter(cmdUser, "@kullaniciAd", DbType.String, ParameterDirection.Input, entity.Username);
                    DBCommandCreator.AddParameter(cmdUser, "@sifre", DbType.String, ParameterDirection.Input, entity.Password);
                    DBCommandCreator.AddParameter(cmdUser, "@kisiID", DbType.Int32, ParameterDirection.Input, id);
                    _rowsAffected = cmdUser.ExecuteNonQuery();
                }
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured while executing Insert() in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }
        public bool Update(User entity)
        {
            _rowsAffected = 0;   
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblKullanici SET sifre = @sifre WHERE kullaniciID = @kullaniciID";
                    DBCommandCreator.AddParameter(cmd, "@sifre", DbType.String, ParameterDirection.Input, entity.Password);
                    DBCommandCreator.AddParameter(cmd, "@kullaniciID", DbType.Int32, ParameterDirection.Input, entity.UserID);
                    _rowsAffected = cmd.ExecuteNonQuery();
                }
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblKisi SET adres = @adres, cepTel = @cepTel, email = @email WHERE kisiID = @kisiID";
                    DBCommandCreator.AddParameter(cmd, "@adres", DbType.String, ParameterDirection.Input, entity.Person.Address);
                    DBCommandCreator.AddParameter(cmd, "@cepTel", DbType.String, ParameterDirection.Input, entity.Person.Phone);
                    DBCommandCreator.AddParameter(cmd, "@email", DbType.String, ParameterDirection.Input, entity.Person.Email);
                    DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, entity.Person.PersonID);
                    _rowsAffected += cmd.ExecuteNonQuery();
                }
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Update() func. in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }

        public List<User> FetchAllCustomers()
        {
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
                                var entity = new User()
                                {
                                    Username = reader.GetString(0),
                                    Role = new Role() { Name = reader.GetInt32(2) == 1 ? "Calisan" : "Musteri", RoleID = reader.GetInt32(2) },
                                    UserID = reader.GetInt32(3),
                                    Person = FetchPersonByID(reader.GetInt32(1))
                                };
                                list.Add(entity);
                            }
                        }
                    }

                }
                return list;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchAllCustomers() in SpiceApp.DataAccessLayer.UserRepository", ex);
            }
            
        }

        

        //USER DETAILS
        private Person FetchPersonByID(int PersonID)
        {
            Person entity = null;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "ad", "soyad", "adres", "cepTel", "email", "ehliyetVerilisTarihi", "dogumTarih", "sirketID", "kisiID" },DBTableNames.Person,
                        "WHERE kisiID = @kisiID");
                    DBCommandCreator.AddParameter(cmd, "@kisiID", DbType.Int32, ParameterDirection.Input, PersonID);
                    CompanyRepository companyRepo = new CompanyRepository();
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                entity = new Person()
                                {
                                    Name = reader.GetString(0),
                                    Surname = reader.GetString(1),
                                    Address = reader.GetString(2),
                                    Phone = reader.GetString(3),
                                    Email = reader.GetString(4),
                                    DriverLicenseDate = reader.GetDateTime(5),
                                    Birthday = reader.GetDateTime(6),
                                    Company = companyRepo.FetchById(reader.GetInt32(7)),
                                    PersonID =  reader.GetInt32(8)
                                };
                            }
                        }
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchPersonByID function, SpiceApp.DataAccessLayer.UserRepository", ex);
            }
        }
        private bool InsertPerson(Person entity)
        {
            try
            {
                using (var cmdPerson = dBConnection.GetSqlCommand())
                {

                    cmdPerson.CommandText = DBCommandCreator.EXEC(new string[] { "ad", "soyad", "adres", "cepTel", "email", "ehliyet", "dogTarih" }, "SP_kisiKayit");

                    DBCommandCreator.AddParameter(cmdPerson, "@ad", DbType.String, ParameterDirection.Input, entity.Name);
                    DBCommandCreator.AddParameter(cmdPerson, "@soyad", DbType.String, ParameterDirection.Input, entity.Surname);
                    DBCommandCreator.AddParameter(cmdPerson, "@email", DbType.String, ParameterDirection.Input, entity.Email);
                    DBCommandCreator.AddParameter(cmdPerson, "@adres", DbType.String, ParameterDirection.Input, entity.Address);
                    DBCommandCreator.AddParameter(cmdPerson, "@cepTel", DbType.String, ParameterDirection.Input, entity.Phone);
                    DBCommandCreator.AddParameter(cmdPerson, "@ehliyet", DbType.Date, ParameterDirection.Input, DateConverter.ToDatabase(entity.DriverLicenseDate));
                    DBCommandCreator.AddParameter(cmdPerson, "@dogTarih", DbType.Date, ParameterDirection.Input, DateConverter.ToDatabase(entity.Birthday));
                    cmdPerson.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
