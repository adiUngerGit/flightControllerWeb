using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Controllers
{
    public class Respone<T>
    {
        public string Status { get; set; }

        public T Data { get; set; }

        public int? ErrorId { get; set; }

        public string Error { get; set; }
        public Respone<T> ResponeSuccess(T data)
        {
            Data = data;
            Error = "Without Error";
            Status = "succses";
            ErrorId = 200;
            return this;
        }
        public Respone<T> error300(T data)
        {
            Data = data;
            Error = "there isnt flights in the current server or in current relative_to time please upload server";
            Status = "failed";
            ErrorId = 300;//error DB
            return this;
        }
        public Respone<T> error500(T data)
        {
            Data = data;
            Error = "there isnt flights in the current server or in current relative_to time please upload server";
            Status = "failed";
            ErrorId = 500;//error DB
            return this;
        }
    }
}

