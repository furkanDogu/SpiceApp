using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiceApp.Models.Entities;
using SpiceApp.Models.Templates;
using SpiceApp.BusinessLayer.Validation;
using SpiceApp.DataAccessLayer.Concretes;
using System.Threading.Tasks;

namespace SpiceApp.BusinessLayer.Concretes
{
    public class ReportBusiness:IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public Response<DailyKmInfo> DailyKmReport(int UserID)
        {
            Response<DailyKmInfo> res = new Response<DailyKmInfo>();
            try
            {
                using (var reportRepository = new ReportRepository())
                {
                    res.Data = reportRepository.DailyKmReport(UserID);

                    if (res.Data.Count > 0)
                    {
                        res.isSuccess = true;
                        res.Message = "Günlük kilometreler raporu başarıyla gösterildi";
                    }
                    else
                    {
                        res.Message = "Günlük kilometre raporu bulunamadı";
                        res.isSuccess = false;
                    }
                }
                return res;
             }
            catch(Exception ex)
            {
                throw new Exception("An error occured in DailyKmReport func in SpiceApp.BusinessLayer.ReportBusiness", ex);
            }
            
        }

        public Response<DailyKmInfo> DailyKmReportByRentID(int UserID, int RentID)
        {
            Response<DailyKmInfo> res = new Response<DailyKmInfo>();
            try
            {
                using(var reportRepository = new ReportRepository())
                {
                    res.Data = reportRepository.DailyKmReportByRentID(UserID, RentID);

                    if(res.Data.Count > 0)
                    {
                        res.isSuccess = true;
                        res.Message = "İlgili aracın kilometre bilgileri gösterildi";
                    }
                    else
                    {
                        res.isSuccess = false;
                        res.Message = "İlgili aracın kilometreleri bulunamadı";
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in DailyKmReportByRentID() func. in SpiceApp.BusinessLayer.ReportBusiness", ex);
            }

        }

        public Response<RentRate> RentRateMonthly(int UserID, DateTime Term)
        {
            Response<RentRate> res = new Response<RentRate>();
            try
            {
                using(var reportRepository = new ReportRepository())
                {
                    res.Data = reportRepository.MonthlyRentRate(UserID, Term);
                    if(res.Data.Count > 0)
                    {
                        res.isSuccess = false;
                        res.Message = "Arabanın aylık kullanım oranları başarıyla döndürüldü";
                    }
                    else
                    {
                        res.isSuccess = false;
                        res.Message = "Arabanın aylık kullanım oranları döndürülürken bir aksilik ile karşılaşıldı";
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in RentDetailMonthly() func in SpiceApp.BusinessLayer.ReportBusiness", ex);
            }
        }

        public Response<OverKmInfo> OverKmInfo(int UserID)
        {
            Response<OverKmInfo> res = new Response<OverKmInfo>();
            try
            {
                using(var reportRepository = new ReportRepository())
                {
                    res.Data = reportRepository.OverKmInfo(UserID);
                    if(res.Data.Count > 0)
                    {
                        res.isSuccess = true;
                        res.Message = "Km aşım oranları gösterilmiştir";
                    }
                    else
                    {
                        res.isSuccess = false;
                        res.Message = "Km aşım oranlarını gösterirken bir sorunla karşılaşılmıştır";
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in OverKmInfo() func. in SpiceApp.BusinessLayer.ReportBusiness", ex);
            }
        }

        public Response<CompanyBalanceInfo> CompanyBalanceInfo(int UserID)
        {
            Response<CompanyBalanceInfo> res = new Response<CompanyBalanceInfo>();
            try
            {
                using (var reportRepository = new ReportRepository())
                {
                    res.Data = reportRepository.CompanyBalanceInfo(UserID);
                    if(res.Data.Count > 0)
                    {
                        res.isSuccess = true;
                        res.Message = "Şirket bakiye bilgileri başarıyla getirilmiştir";
                    }
                    else
                    {
                        res.isSuccess = false;
                        res.Message = "Şirket bakiye bilgilerini getirirken bir sorun ile karşılaşıldı";
                    }
                }
                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in CompanyBalanceInfo() func. in SpiceApp.BusinessLayer.ReportBusiness", ex);
            }
        }





    }
}
