using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiceApp.Models.Entities;
using SpiceApp.BusinessLayer.Validation;
using SpiceApp.DataAccessLayer.Concretes;
using System.Threading.Tasks;

namespace SpiceApp.BusinessLayer.Concretes
{
    public class BrandBusiness : IDisposable
    {
        public Response<Brand> AddBrand(Brand entity)
        {
            Response<Brand> res = new Response<Brand>();
            using(var repo = new BrandRepository())
            {
                res.isSuccess = repo.Insert(entity);

                if (res.isSuccess)
                    res.Message = "Marka başarı ile eklendi";
                else
                    res.Message = "Marka eklenirken bir sorun ile karşılaşıldı";
                return res;

            }
        }

        public Response<Brand> FetchAllBrands()
        {
            Response<Brand> res = new Response<Brand>();
            using(var repo = new BrandRepository())
            {
                
                res.Data = repo.FetchAllBrands();
                if (res.Data.Count > 0)
                {
                    res.Message = "Markalar listelenmiştir";
                    res.isSuccess = true;
                }
                else
                {
                    res.Message = "Markaları listelerken bir hata oluşmuştur";
                    res.isSuccess = false;
                }

                return res;

            }
        }

        public Response<Brand> UpdateBrand(Brand entity)
        {
            Response<Brand> res = new Response<Brand>();
            try
            {
                using (var repo = new BrandRepository())
                {
                    res.isSuccess = repo.Update(entity);
                    if (res.isSuccess)
                        res.Message = "Marka başarıyla güncellendi";
                    else
                        res.Message = "Marka güncellenirken bir sorun ile karşılaşıldı";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in SpiceApp.BusinessLayer.BrandBusiness",ex);
            }
        }

        public Response<Brand> FetchOneBrandById(int BrandID)
        {
            Response<Brand> res = new Response<Brand>();
            res.Data = new List<Brand>();
            try
            {
                Brand temp = null;
                using (var repo = new BrandRepository())
                {
                     temp = repo.FetchById(BrandID);
                }
                if (temp != null)
                {
                    res.Data.Add(temp);
                    res.isSuccess = true;
                    res.Message = "Marka başarıyla getirildi";
                }
                else
                {
                    res.isSuccess = false;
                    res.Message = "Marka getirilirken bir sorun ile karşılaşıldı";
                }
                return res;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchOneBrandById() function in SpiceApp.BusinessLayer.BrandBusiness",ex);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
