using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.Models.Entities;
using SpiceApp.Util.DataUtil;
using System.Data;
using SpiceApp.DataAccessLayer.Interfaces;
using System.Data.SqlClient;


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

            //open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                // creating command text with DBCommandCreator utility class
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "markaID", "marka" }, DBTableNames.Brand, "WHERE markaID = @BrandID");
                DBCommandCreator.AddParameter(cmd, "@BrandID", DbType.Int32, ParameterDirection.Input, BrandID);

                // execute the query
                reader = dBConnection.DataReader(cmd);
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

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing FetchByID() in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public List<Brand> FetchAllBrands()
        {
            // responsible for getting all brands from the db.

            List<Brand> brands = new List<Brand>();

            //open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            try
            {
                // there won't be  a condition in this query so we give String.Empty value to the last parameter.
                cmd.CommandText = DBCommandCreator.SELECT(new string[] { "markaID", "marka" }, DBTableNames.Brand, String.Empty);

                //execute query and get values
                reader = dBConnection.DataReader(cmd);
                if (reader.HasRows)
                {
                    while (reader.Read())
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
                return brands;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllBrands in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                dBConnection.CloseConnection();
            }
        }

        public bool Insert(Brand entity)
        {
            // responsible for inserting new brands to the db.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            //open connection
            SqlCommand cmd = new SqlCommand();
            dBConnection.OpenConnection();

            try
            {
                cmd.CommandText = DBCommandCreator.INSERT(new string[] { "marka" }, DBTableNames.Brand);
                DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);

                //execute query
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                // if added, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Insert() function in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }

        public bool Update(Brand entity)
        {
            // resposible for changing name of the brand in case it was given wrong.

            // clean the attribute. otherwise values left from previous operations may cause conflict
            _rowsAffected = 0;

            //open connection
            dBConnection.OpenConnection();
            SqlCommand cmd = new SqlCommand();

            try
            {
                //create query
                cmd.CommandText = "UPDATE tblMarka SET marka = @marka WHERE markaID = @markaID";
                DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);
                DBCommandCreator.AddParameter(cmd, "@markaID", DbType.Int32, ParameterDirection.Input, entity.BrandID);

                //execute
                _rowsAffected = dBConnection.ExecuteQueries(cmd);

                // if updated, affected rows will be greater than 0
                return _rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Update() func. in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
            finally
            {
                dBConnection.CloseConnection();
            }
        }

    }
}
