using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.DataAccessLayer.Concretes;

namespace SpiceApp.BusinessLayer.Validation
{
    public static class RentDetailValidation
    {
        public static bool ValidateTheScore(int Score)
        {
            // needs to be 1-5
            if (Score < 1 || Score > 5) return false;
            return true;
        }
        public static bool ValiteKmInfo(int KmInfo, int RentID)
        {
            //needs to be more than it was.
            using(var rentRepo = new RentDetailRepository())
            {
                return rentRepo.FetchById(RentID).Car.KmInfo < KmInfo;
            }
        }
    }
}
