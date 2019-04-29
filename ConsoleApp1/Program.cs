using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.ReservationService;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ReservationServiceSoapClient rs = new ReservationServiceSoapClient();
            ResponseOfCar res = rs.FetchAvailableCarsForResv(1006, new DateTime(2019, 04, 29), new DateTime(2019, 5, 1));

            foreach(var car in res.Data.ToList())
            {
                Console.WriteLine(car.CarModel);
            }

            Console.ReadKey();



        }
    }
}
