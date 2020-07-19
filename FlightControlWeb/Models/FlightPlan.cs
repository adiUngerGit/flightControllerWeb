using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
       
        private string FlightPlanId;

        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }

        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("initial_location")]
        public Initial_location Initial_location { get;set; }
        // inside will be longitude, latitude, dataAndTime

        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; }
        // array of directory with longitude latitude, timespanInSeconds

        public string getFlightPlanID()
        {
            return this.FlightPlanId;
        }
        public void setFlightPlanID(string id)
        {
            this.FlightPlanId = id;
        }
    }
}
