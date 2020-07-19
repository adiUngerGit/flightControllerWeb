using FlightControlWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlWeb.middle
{
    public class FlightMiddle
    {
        public List<Flight> GetListFlightFromServer(IProductModel<Server>  Iserver, IProductModel<FlightPlan> Iflight, string relative_to)
        {
            List<Flight> listFlights = new List<Flight>();
            List<Server> listOfServer = Iserver.GetAllProduct().ToList();
            HttpClient client = new HttpClient();
            List<FlightPlan> listFromcurrentServer = Iflight.GetAllProduct().ToList();
            Flight flight = new Flight();
            listFlights = flight.fromPlanToFlight((FlightModel)Iflight, Iflight.GetAllProduct().ToList(), relative_to, false);
            for (int i = 0; i < listOfServer.Count; i++)
            {
                try
                {
                    List<Flight> myDeserializedObjList = listFromServer(listOfServer[i], relative_to);
                    listFlights.AddRange(myDeserializedObjList);
                }
                catch 
                {
                    throw new Exception("error");
                }
            }
            return listFlights;
        }
        public List<Flight> listFromServer(Server s, string relative_to)
        {
            string url = String.Format(s.ServerUrl + "/api/Flights?relative_to=" + relative_to);
            WebRequest requestObject = WebRequest.Create(url);
            requestObject.Method = "GET";
            HttpWebResponse response = null;
            response = (HttpWebResponse)requestObject.GetResponse();
            string test = null;
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                test = sr.ReadToEnd();
                List<Flight> myDeserializedObjList = new List<Flight>();
                myDeserializedObjList = (List<Flight>)Newtonsoft.Json.JsonConvert.DeserializeObject(test, typeof(List<Flight>));
                for (int j = 0; j < myDeserializedObjList.Count; j++)
                {
                    myDeserializedObjList[j].Is_External = true;
                }
                sr.Close();
                return myDeserializedObjList;
            }
        }
    }
}
