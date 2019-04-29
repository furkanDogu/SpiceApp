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
    public class RentDetailBusiness: IDisposable
    {
        public Response<RentDetail> CompleteReservation(int id)
        {
            // this function only will be called by the employee.
            Response<RentDetail> res = new Response<RentDetail>();
            try
            {
                using (var repo = new RentDetailRepository())
                {
                    res.isSuccess = repo.Insert(new RentDetail() { RentID = id });
                    if (res.isSuccess)
                        res.Message = "Rezervasyon başarıyla tamamlandı ve araba teslim edildi";
                    else
                        res.Message = "Rezervasyon tamamlanırken bir hata oluştu";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in CompleteReservation() func. in SpiceApp.BusinessLayer.ReservationBusiness", ex);
            }
        }

        public Response<RentDetail> FetchAllRentDetailById(int id)
        {
            Response<RentDetail> res = new Response<RentDetail>();
            try
            {
                using (var repo = new RentDetailRepository())
                {
                    res.Data = repo.FetchAllRentDetail(id);
                    res.isSuccess = true;
                    res.Message = "Kira detayları listelendi";
                    
                }
                return res;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchAllRentDetailById() function in SpiceApp.BusinessLayer.RentDetailBusiness", ex);
            }
        }

        public Response<RentDetail> FetchOneById(int id)
        {
            Response<RentDetail> res = new Response<RentDetail>();
            try
            {
                using(var rentDetailRepo = new RentDetailRepository())
                {
                    res.Data = new List<RentDetail>();
                    res.Data.Add(rentDetailRepo.FetchById(id));
                    if(res.Data != null)
                    {
                        res.isSuccess = true;
                        res.Message = "Kira detay bilgisi gösteriliyor";
                    }
                    else
                    {
                        res.isSuccess = false;
                        res.Message = "Kira detay bilgisi gösterilirken bir sorun ile karşılaşıldı";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw new Exception("An error occured in FetchOneById() function in SpiceApp.BusinessLayer.RentDetailBusiness", ex);
            }
        }

        public Response<RentDetail> ReturnCarToCompany(int RentID, int KmInfo, int Score) 
        {
            Response<RentDetail> res = new Response<RentDetail>();

            // check if the score in 1-5 range and km info is valid 
            if (RentDetailValidation.ValidateTheScore(Score) && RentDetailValidation.ValiteKmInfo(KmInfo, RentID)) 
            {
                try
                {
                    using (var repo = new RentDetailRepository())
                    {
                        res.isSuccess = repo.ReturnCarToCompany(RentID, KmInfo, Score);
                        if (res.isSuccess)
                            res.Message = "Araç başarıyla firmaya geri döndürüldü";
                        else
                            res.Message = "Araç firmaya geri döndürülürken bir aksilik meydana geldi";
                    }
                    return res;
                }
                catch (Exception ex)
                {

                    throw new Exception("An error occured in ReturnCarToCompany() func. in SpiceApp.BusinessLayer.RentDetailBusiness", ex);
                }
            } else
            {
                res.isSuccess = false;
                res.Message = "Geçersiz parametreler";
            }
            return res;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

       
    }
}
