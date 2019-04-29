using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.BusinessLayer.Validation
{
    public static class DateValidation
    {
        public static bool CheckIfValid(DateTime startingDate, DateTime endTime)
        {   bool result = true;
            if (startingDate > endTime) // is the starting date less than end date 
                result = false;
            else if (startingDate < DateTime.Now.Date)  // is the starting date less than today's date
                result = false;
            else if (endTime <= DateTime.Now.Date) // is the end date less than today's date
                result = false;

            return result;             
        }
    }
}
