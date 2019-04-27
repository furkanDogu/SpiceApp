using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Util.DateUtil
{
    public static class DateConverter
    {
        public static string ToDatabase(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static string ToClient(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }
    }
}
