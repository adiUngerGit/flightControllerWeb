using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class GenerateId
    {
        //string id;
        
        public string Generate()
        {
            var rand = new Random();
            DateTime date =  System.DateTime.Now;
            double day = Double.Parse(date.Day.ToString());
            double year = Double.Parse(date.Year.ToString());
            double hour = Double.Parse(date.Hour.ToString());
            double seconds = Double.Parse(date.Second.ToString());
            double months = Double.Parse(date.Month.ToString());

            int randomNum1 = rand.Next(0, 26); // Zero to 25
            char firstChar = (char)('a' + randomNum1);
            string first = (day + hour).ToString();
            string second = (months + seconds).ToString();
            int randomNum2 = rand.Next(0, 26); // Zero to 25
            char secondChar = (char)('a' + randomNum2);

            return first +firstChar+ second + secondChar;
        }
    }

 
    //double time = DateTime.Now.Ticks;
    //id = time.ToString("x");
    //long i = 1;
    //foreach (byte b in Guid.NewGuid().ToByteArray())
    //{
    //   i *= (int)b + 1;

    // }
    // id =  String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
    // return id;

}
