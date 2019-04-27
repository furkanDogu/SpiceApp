using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.Models.Entities
{
    public class Response<T> where T : class
    {
        public Response()
        {

        }
        public bool isSuccess { get; set; }
        public List<T> Data { get; set; }
        public string Message { get; set; }
    }
}
