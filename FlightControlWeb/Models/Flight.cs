using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using FlightControlWeb.DB;
namespace FlightControlWeb.Models
{

    public class Flight
    {
        [JsonPropertyName("flight_id")]
        public string Flight_ID { get; set; }
        [JsonPropertyName("company_name")]
        public string Company_Name { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("date_time")]
        public string Date_Time { get; set; }
        [JsonPropertyName("is_external")]
        public bool Is_External { get; set; }

        //make the list of flights
        public List<Flight> fromPlanToFlight(FlightModel flightModel, List<FlightPlan> planList, string data_timeNow, bool isExternal)
        {

            List<Flight> flightsList = new List<Flight>();
            for (int i = 0; i < planList.Count; i++)
            {
                Flight flight = new Flight();

                double startLngD = 0, statrLatD = 0;
                string dateAndTimeStart = planList[i].Initial_location.date_time;
                string startlng = planList[i].Initial_location.Longitude.ToString();
                string startlat = planList[i].Initial_location.Latitude.ToString();

                flight.Company_Name = planList[i].Company_Name;
                flight.Flight_ID = planList[i].getFlightPlanID();
                flight.Passengers = planList[i].Passengers;
                flight.Date_Time = dateAndTimeStart;
                flight.Is_External = isExternal;
                try
                {
                    startLngD = Double.Parse(startlng);
                    statrLatD = Double.Parse(startlat);
                }
                catch
                {
                    throw new Exception("error in parse double in class flight");
                }
                FlightsDataBase flightDataBase = flightModel.getDB();
                SegmentDataBase segmentDataBase = flightDataBase.getSegmentDB();
                List<Segment> listSegment = segmentDataBase.GetAllSegmentByID(flight.Flight_ID);

                double sumSegment = 0, endLat = -1, endLong = -1, timeSpan = 0;

                //if not enter relative to
                if (data_timeNow == null)
                {
                    data_timeNow = dateAndTimeStart;
                }

                //found the long and lat for the flight
                try
                {
                    Dictionary<string, int> splitDataNow = splitTime(data_timeNow);
                    Dictionary<string, int> splitDataStart = splitTime(dateAndTimeStart);
                    int sumDataSecond = fromTimeToSecond(splitDataNow, splitDataStart);
                    bool enter = true;

                    //for all the sigment in the DB
                    for (int j = 0; j < listSegment.Count; j++)
                    {
                        for (int x = 0; x < listSegment.Count; x++)
                        {
                            if (listSegment[j].Index == j)
                            {
                                sumSegment = sumSegment + listSegment[j].timespan_seconds;
                                //is this is the segment that we nees to take the info from there
                                if (sumSegment >= sumDataSecond)
                                {
                                    endLat = listSegment[j].Latitude;
                                    endLong = listSegment[j].Longitude;
                                    timeSpan = sumSegment;
                                    //the start time is the last of the kast segment
                                    if (j > 0)
                                    {
                                        statrLatD = listSegment[j - 1].Latitude;
                                        startLngD = listSegment[j - 1].Longitude;

                                    }
                                    enter = false;
                                }
                                break;
                            }
                        }
                        if (!enter)
                        {
                            break;
                        }
                    }
                    //if not error, enter this flight to List  
                    if (endLat > -1 && endLong > -1)
                    {
                        Dictionary<string, double> longAndLat = getLocation(statrLatD, startLngD,
                            endLat, endLong, timeSpan, dateAndTimeStart, data_timeNow);
                        if (flight.Latitude > -1 && flight.Longitude > -1)
                        {
                            flight.Latitude = longAndLat["lat"];
                            flight.Longitude = longAndLat["long"];
                            flightsList.Add(flight);
                        }
                    }
                }
                catch
                {
                    throw new Exception("error in split time");
                }
            }
            return flightsList;
        }

        public int fromTimeToSecond(Dictionary<string, int> mapTimeNow, Dictionary<string, int> mapTimeStart)
        {
            //time per second from start to now
            int secondFromStartToNow = 0;
            secondFromStartToNow += 12 * 30 * 24 * 60 * 60 * (mapTimeNow["year"] - mapTimeStart["year"]);
            secondFromStartToNow += 30 * 24 * 60 * 60 * (mapTimeNow["month"] - mapTimeStart["month"]);
            secondFromStartToNow += 24 * 60 * 60 * (mapTimeNow["day"] - mapTimeStart["day"]);
            //todo time zone
            secondFromStartToNow += 60 * 60 *
                ((mapTimeNow["hour"] - mapTimeNow["time zone"]) - (mapTimeStart["hour"] - mapTimeStart["time zone"]));
            secondFromStartToNow += 60 * (mapTimeNow["minutes"] - mapTimeStart["minutes"]);
            secondFromStartToNow += (mapTimeNow["second"] - mapTimeStart["second"]);

            return secondFromStartToNow;
        }
        public static Dictionary<string, int> splitTime(string time)
        {
            time.Replace("<", "");
            time.Replace(">", "");
            Dictionary<string, int> mapTime = new Dictionary<string, int>();

            string[] split = null, splitDate = null, splitTime = null;
            // TODO TIME ZONE
            split = time.Split("Z");
            if (split[1] != "")
            {
                mapTime["time zone"] = int.Parse(split[1]);
            }
            else
            {
                mapTime["time zone"] = 0;
            }
            split = split[0].Split("T");
            splitTime = split[0].Split("-");
            splitDate = split[1].Split(":");
            mapTime["year"] = int.Parse(splitTime[0]);
            mapTime["month"] = int.Parse(splitTime[1]);
            mapTime["day"] = int.Parse(splitTime[2]);
            mapTime["hour"] = int.Parse(splitDate[0]);
            mapTime["minutes"] = int.Parse(splitDate[1]);
            mapTime["second"] = Convert.ToInt32(Convert.ToDouble(splitDate[2]));

            return mapTime;
        }

        public Dictionary<string, double> getLocation(double startLat, double startLong,
           double endLat, double endLong, double timespanSeconds, string timeStart, string timeNow)
        {

            // split time
            Dictionary<string, int> mapTimeNow = splitTime(timeNow);
            Dictionary<string, int> mapTimeStart = splitTime(timeStart);

            //distance: Length of track
            double distX = startLat - endLat;
            double distY = startLong - endLong;
            double distance = Math.Sqrt(distX * distX + distY * distY);

            //Amount of time per second
            double velocity = distance / timespanSeconds;

            double secondFromStartToNow = fromTimeToSecond(mapTimeNow, mapTimeStart);
            //distance from start to now
            double distaceFrimStartToNow = velocity * secondFromStartToNow;
            if (distaceFrimStartToNow > distance)
            {
                Dictionary<string, double> endMapErorr = new Dictionary<string, double>();
                endMapErorr["long"] = -1;
                endMapErorr["lat"] = -1;
                return endMapErorr;
            }
            if (distaceFrimStartToNow == 0)
            {
                Dictionary<string, double> endMapZ = new Dictionary<string, double>();
                endMapZ["long"] = startLong;
                endMapZ["lat"] = startLat;
                return endMapZ;
            }
            //Percent
            double PercentFromAllDis = distance / distaceFrimStartToNow;

            //finall
            double latNow = startLat + ((endLat - startLat) * (1 / PercentFromAllDis));
            double longNow = startLong + ((endLong - startLong) * (1 / PercentFromAllDis));

            Dictionary<string, double> endMap = new Dictionary<string, double>();
            endMap["long"] = longNow;
            endMap["lat"] = latNow;

            return endMap;
        }
    }
}