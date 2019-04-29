using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data;
using SpiceApp.DataAccessLayer.Interfaces;


namespace SpiceApp.DataAccessLayer.Concretes
{
    public class BrandRepository : IUpdatableRepository<Brand>, IDisposable
    {
        private DBConnection dBConnection;
        private int _rowsAffected;

        public BrandRepository()
        {
            dBConnection = new DBConnection();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool DeleteById(int BrandID)
        {
            throw new NotImplementedException();
        }

        public Brand FetchById(int BrandID)
        {
            // responsible for getting the brand with the given id.

            Brand entity = null;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    // creating command text with DBCommandCreator utility class
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "markaID", "marka" }, DBTableNames.Brand, "WHERE markaID = @BrandID");
                    DBCommandCreator.AddParameter(cmd, "@BrandID", DbType.Int32, ParameterDirection.Input, BrandID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // creating brand instance with fetched values
                                entity = new Brand()
                                {
                                    BrandID = reader.GetInt32(0),
                                    BrandName = reader.GetString(1)
                                };
                            }
                        }
                    }
                }
                return entity;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while executing FetchByID() in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
        }

        public List<Brand> FetchAllBrands()
        {
            // responsible for getting all brands from the db.

            List<Brand> brands = new List<Brand>();
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    // there won't be  a condition in this query so we give String.Empty value to the last parameter.
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] {"markaID", "marka"}, DBTableNames.Brand, String.Empty);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                // create brand instance
                                var entity = new Brand()
                                {
                                    BrandID = reader.GetInt32(0),
                                    BrandName = reader.GetString(1)
                                };
                                //add it to the brand list
                                brands.Add(entity);
                            }
                        }
                    }
                    return brands;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchAllBrands in SpiceApp.DataAccessLayer.BrandRepository",ex);
            }
        }

        public bool Insert(Brand entity)
        {
            // responsible for inserting new brands to the db.
            
            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.INSERT(new string[] {"marka"},DBTableNames.Brand);
                    DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);
                    _rowsAffected = cmd.ExecuteNonQuery();

                    // if added, affected rows will be greater than 0
                    return _rowsAffected > 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Insert() function in SpiceApp.DataAccessLayer.BrandRepository",ex);
            }
        }

        public bool Update(Brand entity)
        {
            // resposible for changing name of the brand in case it was given wrong.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblMarka SET marka = @marka WHERE markaID = @markaID";
                    DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);
                    DBCommandCreator.AddParameter(cmd, "@markaID", DbType.Int32, ParameterDirection.Input, entity.BrandID);

                    _rowsAffected = cmd.ExecuteNonQuery();

                    // if updated, affected rows will be greater than 0
                    return _rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Update() func. in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
        }

    }
}
