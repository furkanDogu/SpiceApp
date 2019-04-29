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
    public class ReservationBusiness: IDisposable
    {
        public Response<Car> FetchAvailableCarsForResv(int UserID, DateTime startingDate, DateTime endTime)
        {
            try
            {
                Response<Car> res = new Response<Car>();
                if (DateValidation.CheckIfValid(startingDate, endTime)) // check the dates if they are valid (more detail in the function definition)
                {
                    using (var repo = new ReservationRepository())
                    {
                        res.Data = repo.FetchAvailableCarsForResv(UserID, startingDate, endTime);
                        
                        if (res.Data.Count > 0)
                        {
                            res.Message = "Rezervasyon için uygun olan arabaların listesi";
                            res.isSuccess = true;
                        }
                        else
                        {
                            res.Message = "Rezervasyona uygun araçları getirirken bir hata ile karşılaşıldı";
                            res.isSuccess = false;
                        }
                        return res;
                    }
                }
                else
                {
                    res.Message = "Geçersiz rezervasyon tarihleri !";
                    res.isSuccess = false;
                }
                return res;

            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchAvailableCarsForResv() in SpiceApp.BusinessLayer.CarBusiness", ex);
            }

        }
        public Response<Reservation> FetchAllResvById(int UserID)
        {
            Response<Reservation> res = new Response<Reservation>();
            List<Reservation> list = new List<Reservation>();
            try
            {
                using(var repo = new ReservationRepository())
                {
                    list = repo.FetchAllByUserId(UserID);
                    res.Data = list;
                    if(list.Count > 0)
                    {
                        res.Message = "Rezervasyonlar başarıyla listelenmiştir";
                        res.isSuccess = true;
                    }
                    else
                    {

                        res.Message = "Rezervasyonları listelerken bir sorun ile karşılaşıldı";
                        res.isSuccess = false;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchAllResvById() function in SpiceApp.BusinessLayer.ReservationBusiness",ex);
            } 
        }
        public Response<Reservation> MakeReservation(int CarID, int UserID, DateTime startingDate, DateTime endTime)
        {
            try
            {
                Reservation reservation = new Reservation() { Car = new Car() { CarID = CarID}, User = new User() { UserID = UserID}, StartingDate = startingDate, EndDate = endTime };
                Response<Reservation> res = new Response<Reservation>();
                if (DateValidation.CheckIfValid(startingDate, endTime))  // check the dates if they are valid (more detail in the function definition)
                {
                    using (var repo = new ReservationRepository())
                    {
                        res.isSuccess = repo.Insert(reservation);

                        if (res.isSuccess)
                            res.Message = "Rezervasyon işlemi başarı ile gerçekleştirilmiştir";
                        else
                            res.Message = "Rezervasyon işlemi gerçekleştirilirken bir sorun ile karşılaşılmıştır";
                        return res;
                    }
                }
                else
                {
                    res.isSuccess = false;
                    res.Message = "Geçersiz rezervasyon tarihleri !";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in MakeReservation() function in SpiceApp.BusinessLayer.CarBusiness", ex);
            }

        }
        public Response<Reservation> CancelReservation(int ReservationID)
        {
            try
            {
                Response<Reservation> res = new Response<Reservation>();

                using (var repo = new ReservationRepository())
                {
                    res.isSuccess = repo.DeleteById(ReservationID);
                    if (res.isSuccess)
                        res.Message = "Rezervasyon başarıyla iptal edildi";
                    else
                        res.Message = "Rezervasyon iptal edilirken bir sorun ile karşılaşıldı";
                }
                return res;
            } catch(Exception ex)
            {
                throw new Exception("An error occured in CancelReservation() func. in SpiceApp.BusinessLayer.ReservationBusiness", ex);
            }
            
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
