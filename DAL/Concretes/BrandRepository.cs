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
            Brand entity = null;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    string condition = "WHERE markaID = @BrandID";
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] { "markaID", "marka" }, DBTableNames.Brand, condition);
                    DBCommandCreator.AddParameter(cmd, "@BrandID", DbType.Int32, ParameterDirection.Input, BrandID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                entity = new Brand()
                                {
                                    BrandID = reader.GetInt32(0),
                                    BrandName = reader.GetString(1)
                                };
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                throw new Exception("An error occured while executing FetchByID() in SpiceApp.DataAccessLayer.BrandRepository", ex);
            }
            

            return entity;
        }

        public List<Brand> FetchAllBrands()
        {
            List<Brand> brands = new List<Brand>();
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.SELECT(new string[] {"markaID", "marka"}, DBTableNames.Brand, String.Empty);
                    
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                var entity = new Brand()
                                {
                                    BrandID = reader.GetInt32(0),
                                    BrandName = reader.GetString(1)
                                };

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
            _rowsAffected = 0;
            try
            {
                using(var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = DBCommandCreator.INSERT(new string[] {"marka"},DBTableNames.Brand);
                    DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);
                    _rowsAffected = cmd.ExecuteNonQuery();

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
            _rowsAffected = 0;
            try
            {
                using (var cmd = dBConnection.GetSqlCommand())
                {
                    cmd.CommandText = "UPDATE tblMarka SET marka = @marka WHERE markaID = @markaID";
                    DBCommandCreator.AddParameter(cmd, "@marka", DbType.String, ParameterDirection.Input, entity.BrandName);
                    DBCommandCreator.AddParameter(cmd, "@markaID", DbType.Int32, ParameterDirection.Input, entity.BrandID);

                    _rowsAffected = cmd.ExecuteNonQuery();

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
