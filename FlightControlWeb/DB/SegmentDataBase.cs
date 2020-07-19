using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;

using System.Data;
using FlightControlWeb.Models;
using System.IO;


namespace FlightControlWeb
{
    public class SegmentDataBase
    {
        private string createTableQuery = @"CREATE TABLE IF NOT EXISTS [SegmentTabel] (
                          [ID] VARCHAR(2048) NULL,
                          [in] VARCHAR(2048) NULL,
                          [Aindex] DOUBLE(2048,8) NULL,
                          [latitude] DOUBLE(2048,8) NULL,
                          [longitude] DOUBLE(2048,8) NULL,
                          [timespan_seconds] DOUBLE(2048,8) NULL

                          )";
        public SegmentDataBase()
        {
            System.Data.SQLite.SQLiteConnection.CreateFile("SegmentDataBaseFile2.db3");        // Create the file which will be hosting our database
        }
        public void AddTODB(string id, double lon, double lat, double timespan, double _index)
        {
            // This is the query which will create a new table in our database file with three columns. An auto increment column called "ID", and two NVARCHAR type columns with the names "Key" and "Value"
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=SegmentDataBaseFile2.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database

                    com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();
                    //  com.CommandText = "INSERT INTO SegmentTabel(index) Values ("+ _index + ")";
                    //com.ExecuteNonQuery();      // Execute the query
                    //string index = 5;
                    com.CommandText = "INSERT INTO SegmentTabel(ID,longitude,latitude,Aindex ,timespan_seconds)" +
                        " Values ('" + id + "'," + lon + "," + lat + "," + _index + "," + timespan + ")";
                    com.ExecuteNonQuery();      // Execute the query
                    con.Close();        // Close the connection to the database
                }
            }
        }
        public double getInfo(string ID, string info)
        {
            double value;
            // This is the query which will create a new table in our database file with three columns. An auto increment column called "ID", and two NVARCHAR type columns with the names "Key" and "Value"
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=SegmentDataBaseFile2.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database

                    com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();

                    com.CommandText = " SELECT* FROM " + info + "WHERE ID = '" + ID + "'";
                    value = Convert.ToInt32(com.ExecuteScalar());
                    con.Close();
                }
                return value;
            }
        }
        public List<Segment> GetAllSegmentByID(string ID)
        {
            //System.Data.SQLite.SQLiteConnection.CreateFile("databaseFile.db3");        // Create the file which will be hosting our database
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=SegmentDataBaseFile2.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    List<Segment> SegmentList = new List<Segment>();
                    con.Open();
                   // com.CommandText = "Select * FROM SegmentTabel";
                    com.CommandText = "Select * FROM SegmentTabel WHERE ID = '" + ID + "'";      // Select all rows from our database table
                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Segment segment = new Segment();
                            try
                            {
                                segment.Latitude = double.Parse(reader["latitude"].ToString());
                                segment.Longitude = double.Parse(reader["longitude"].ToString());
                                segment.timespan_seconds = double.Parse(reader["timespan_seconds"].ToString());
                                segment.Index = double.Parse(reader["Aindex"].ToString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);                           }

                            SegmentList.Add(segment);
                        }
                        reader.Close();
                    }
                    
                    con.Close();
                    return SegmentList;
                }
            }
        }

        public void delete(string ID)
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=SegmentDataBaseFile2.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();
                    string deleteString = @"delete from Segments where ID = " + ID;
                    com.CommandText = deleteString;
                    com.Connection = con;
                    com.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
}


