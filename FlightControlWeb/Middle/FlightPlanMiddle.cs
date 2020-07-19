using FlightControlWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlightControlWeb.middle
{
    public class FlightPlanMiddle
    {
        public FlightPlan getFlightsFromServer(Server s, string id)
        {
            string url = s.ServerUrl + "/api/FlightPlan/" + id;
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = null;
                response = (HttpWebResponse)request.GetResponse();
                string test = null;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    test = sr.ReadToEnd();
                    FlightPlan myDeserializedObjList =
                        (FlightPlan)Newtonsoft.Json.JsonConvert.DeserializeObject(test, typeof(FlightPlan));
                    myDeserializedObjList.setFlightPlanID(id);
                    sr.Close();
                    return myDeserializedObjList;
                }
            }
            catch
            {
                throw new Exception("faild to load from server");
            }
        }
    }
}
