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
    public class CarBusiness : IDisposable
    {
        public Response<Car> FetchAllCarsByCompany(int CompanyID)
        {
            Response<Car> res = new Response<Car>();
            try
            {
                var cars = new List<Car>();

                using (var repo = new CarRepository())
                {

                    cars = (List<Car>)repo.FetchAllByCompany(CompanyID);
                }
                res.Data = cars;
                if(cars.Count != 0)
                {
                    res.isSuccess = true;
                    res.Message = "Şirketinize ait bütün arabaların listesi";
                }
                else
                {
                    res.isSuccess = false;
                    res.Message = "İlgili şirket ID'sine ait araba bulunamadı";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing FetchAllCars() in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public Response<Car> AddNewCar(Car entity)
        {
            Response<Car> res = new Response<Car>();
            try
            {
                using (var repo = new CarRepository())
                {
                    res.isSuccess = repo.Insert(entity);
                    if (res.isSuccess)
                        res.Message = "Araba başarıyla eklendi";
                    else 
                        res.Message = "Araba eklenirken bir sorun ile karşılaşıldı";
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while executing AddNewCar() in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public Response<Car> FetchCarById(int CarID)
        {
            Response<Car> res = new Response<Car>();
            try
            {
                using (var repo = new CarRepository())
                {
                    res.Data = new List<Car>();
                    res.Data.Add(repo.FetchById(CarID));
                    if(res.Data.Count > 0)
                    {
                        res.Message = "İlgili arabanın detayları ";
                        res.isSuccess = true;
                    }
                    else
                    {
                        res.Message = "İlgili ID'ye sahip araba bulunamadı";
                        res.isSuccess = false;
                    }
                    return res;
                }

            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while executing FetchCarById() in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public Response<Car> DeleteCarById(int CarID)
        {
            Response<Car> res = new Response<Car>();

            try
            {
                using(var repo = new CarRepository())
                {
                    res.isSuccess = repo.DeleteById(CarID);
                    if (res.isSuccess)
                        res.Message = "Araç başarıyla silindi";
                    else
                        res.Message = "Araç silinirken bir sorun ile karşılaşıldı";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in DeleteCarById() function in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public Response<Car> ActivateCarById(int CarID)
        {
            Response<Car> res = new Response<Car>();
            try
            {
                using(var repo = new CarRepository())
                {
                    res.isSuccess = repo.ReActivateCarById(CarID);
                    if (res.isSuccess)
                        res.Message = "Araba başarıyla aktifleştirildi";
                    else
                        res.Message = "Araba aktifleştirilirken bir sorun oluştu";
                }
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in ActivateCarById() function in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public Response<Car> UpdateCar(Car entity)
        {
            Response<Car> res = new Response<Car>();
            try
            {
                using(var repo = new CarRepository())
                {
                    res.isSuccess = repo.Update(entity);
                    if (res.isSuccess)
                        res.Message = "Araba bilgileri başarı ile güncellendi";
                    else
                        res.Message = "Arabayı güncellerken bir sorun meydana geldi";
                }
                return res;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in UpdateCar() function in SpiceApp.BusinessLayer.CarBusiness", ex);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
