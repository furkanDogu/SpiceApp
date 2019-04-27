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
            if (startingDate > endTime) // is the first date less than second date 
                result = false;
            else if (startingDate < DateTime.Now.Date)  // is the first date less than second date
                result = false;
            else if (endTime <= DateTime.Now.Date)
                result = false;

            return result;             
        }
    }
}
