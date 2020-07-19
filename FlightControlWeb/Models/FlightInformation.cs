using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace FlightControlWeb.Models
{
    public class FlightInformation
    {
        public class flightInformation
        {
            public static Dictionary<string, int> splitTime(string time)
            {
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
                mapTime["second"] = int.Parse(splitDate[2]);

                return mapTime;
            }
            public Dictionary<string, double> getLocation(string flightId, string timeNow)
            {
                //acording id
                double startLat = 1, startLong = 3, endLat = 1, endLong = 13, timespanSeconds = 10;
                string timeStart = "2020-12-26T23:56:21Z";

                // split time
                Dictionary<string, int> mapTimeNow = splitTime(timeNow);
                Dictionary<string, int> mapTimeStart = splitTime(timeStart);

                //distance: Length of track
                double distX = startLat - endLat;
                double distY = startLong - endLong;
                double distance = Math.Sqrt(distX * distX + distY * distY);

                //Amount of time per second
                double velocity = distance / timespanSeconds;

                //time per second from start to now
                double secondFromStartToNow;
                secondFromStartToNow = 12 * 30 * 24 * 60 * 60 * (mapTimeNow["year"] - mapTimeStart["year"]);
                secondFromStartToNow = 30 * 24 * 60 * 60 * (mapTimeNow["month"] - mapTimeStart["month"]);
                secondFromStartToNow = 24 * 60 * 60 * (mapTimeNow["day"] - mapTimeStart["day"]);
                secondFromStartToNow = 60 * 60 *
                    ((mapTimeNow["hour"] - mapTimeNow["time zone"]) - (mapTimeStart["hour"] - mapTimeStart["time zone"]));
                secondFromStartToNow = 60 * (mapTimeNow["minutes"] - mapTimeStart["minutes"]);
                secondFromStartToNow = (mapTimeNow["second"] - mapTimeStart["second"]);

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
}
