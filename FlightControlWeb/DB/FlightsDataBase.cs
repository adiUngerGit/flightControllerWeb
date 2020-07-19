using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;

using System.Data;
using FlightControlWeb.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.DB
{
    public class FlightsDataBase : DbContext
    {
        private SegmentDataBase segmentDB;
        private string createTableQuery = @"CREATE TABLE IF NOT EXISTS [MyTable] (
                          [FlightID] VARCHAR(2048) NULL PRIMARY KEY,
                          [Passenger] INTEGER(2048) NULL,
                          [DataAndTime] VARCHAR(2048) NULL,
                          [FlightCompany] TEXT(2048) NULL,
                          [Longitude] DOUBLE(2048,8) NULL,
                          [Latitude] DOUBLE(2048,8) NULL
                          )";
        public FlightsDataBase()
        {
            segmentDB = new SegmentDataBase();
            System.Data.SQLite.SQLiteConnection.CreateFile("databaseFile.db3");        // Create the file which will be hosting our database
        }
        public SegmentDataBase getSegmentDB()
        {
            return segmentDB;
        }
        public void AddTODB(FlightPlan flightPlan)
        {
            // This is the query which will create a new table in our database file with three columns. An auto increment column called "ID", and two NVARCHAR type columns with the names "Key" and "Value"
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=databaseFile.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database
                    com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();                  // Execute the query
                                                            // Add the first entry into our database
                    GenerateId generate = new GenerateId();
                    string id = generate.Generate();
                    //flightPlan.setFlightPlanID(id);
                    while (checkExists(id, com))
                    {
                        id = generate.Generate();
                    }
                    flightPlan.setFlightPlanID(id);

                    double valuelat, valuelng;
                    string dateAndTime;
                    valuelat = flightPlan.Initial_location.Latitude;
                    
                    
                    valuelng = flightPlan.Initial_location.Longitude;
                    dateAndTime = flightPlan.Initial_location.date_time;
                    com.CommandText = "INSERT INTO MyTable(FlightID, Passenger, DataAndTime, FlightCompany,Longitude, Latitude) Values ('"
                      + id + "'," + flightPlan.Passengers.ToString() + ",'" + dateAndTime + "','" +
                      flightPlan.Company_Name + "'," + valuelng + "," + valuelat + ")";

                    for (int i = 0; i < flightPlan.Segments.Count; i++)
                    {
                        double timespan_seconds = flightPlan.Segments[i].timespan_seconds;
                        //("timespan_seconds", out double timespan_seconds);
                        double latitude = flightPlan.Segments[i].Latitude;
                        double longitude = flightPlan.Segments[i].Longitude;
                        //flightPlan.Segments[i].TryGetValue("latitude", out double latitude);
                        //flightPlan.Segments[i].TryGetValue("longitude", out double longitude);
                        segmentDB.AddTODB(id, longitude, latitude, timespan_seconds, (double)i);

                    }
                    com.ExecuteNonQuery();      // Execute the query
                    con.Close();        // Close the connection to the database
                }
            }
        }

        public bool checkExists(string id, SQLiteCommand com)
        {
            bool exists = false;
            try
            {
                com.CommandText = "SELECT count(*) FROM MyTable WHERE FlightID='" + id + "'";
                int count = Convert.ToInt32(com.ExecuteScalar());
                if (count != 0)
                {
                    exists = true;
                }
            }
            catch
            {
                exists = false;
            }
            return exists;
        }

        public void DeleteFromDB(string flightID)
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=databaseFile.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    try
                    {
                        con.Open();
                        string deleteString = @"delete from MyTable where FlightID = '" + flightID + "'";
                        com.CommandText = deleteString;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                        con.Close();
                    }
                    catch
                    {
                        throw new Exception("error in delete flight from db");
                    }
                }
            }

        }


        public List<FlightPlan> ReadFromDB()
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=databaseFile.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    List<FlightPlan> flightsList = new List<FlightPlan>();
                    con.Open();
                    com.CommandText = "Select * FROM MyTable";      // Select all rows from our database table
                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FlightPlan flight = new FlightPlan();
                            //     if (rdr[0].Equals(id))
                            //   {
                            flight.setFlightPlanID(reader["FlightID"].ToString());
                            flight.Company_Name = reader["FlightCompany"].ToString();
                            // flight.Initial_location = new Dictionary<string, string>()
                            // {
                            //    { "DataAndTime" ,reader["DataAndTime"].ToString() },
                            //  { "Longitude" ,reader["longitude"].ToString()  },
                            //{ "Latitude" , reader["latitude"].ToString() }
                            //};
                            double d = Convert.ToDouble(reader["Longitude"]);
                            flight.Initial_location = new Initial_location();
                            flight.Initial_location.Longitude = d;
                            flight.Initial_location.date_time = reader["DataAndTime"].ToString();
                            flight.Initial_location.Latitude = Convert.ToDouble(reader["Latitude"]);

                            try
                            {
                                flight.Passengers = int.Parse(reader["Passenger"].ToString());

                            }
                            catch
                            {
                                reader.Close();
                                throw new Exception("error in reading all flights from db");
                            }
                            flightsList.Add(flight);
                        }
                        reader.Close();

                    }

                    con.Close();
                    return flightsList;
                }
            }
        }

        public FlightPlan GetFlightPlan(string ID)
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=databaseFile.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    try
                    {
                        FlightPlan flightPlan = new FlightPlan();
                        con.Open();
                        if (!checkExists(ID, com))
                        {
                            throw new Exception();
                        }
                        com.CommandText = "Select * FROM MyTable WHERE FlightID = '" + ID + "'";

                        using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                flightPlan.Company_Name = reader["FlightCompany"].ToString();
                                flightPlan.Passengers = int.Parse(reader["Passenger"].ToString());
                                flightPlan.Initial_location = new Initial_location();
                                flightPlan.Initial_location.date_time = reader["DataAndTime"].ToString();
                                flightPlan.Initial_location.Longitude = Convert.ToDouble(reader["Longitude"]);
                                flightPlan.Initial_location.Latitude = Convert.ToDouble(reader["Latitude"]);
                                flightPlan.Segments = segmentDB.GetAllSegmentByID(ID);





                            }
                            reader.Close();
                        }
                        con.Close();
                        return flightPlan;
                    }
                    catch
                    {
                        throw new Exception("error in get Flight By id");
                    }
                }
            }
        }
    }
}