using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;

using System.Data;
using FlightControlWeb.Models;
using System.IO;


namespace FlightControlWeb.DB
{
    public class ServerDataBase
    {
        private string createTableQuery = @"CREATE TABLE IF NOT EXISTS [ServerTable] (
                          [ServerID] VARCHAR(2048) NULL PRIMARY KEY,
                          [ServerURL] VARCHAR(2048) NULL
                          )";
        public ServerDataBase()
        {
            System.Data.SQLite.SQLiteConnection.CreateFile("ServerDB.db3");        // Create the file which will be hosting our database
        }
        public void AddTODB(Server server)
        {
            // This is the query which will create a new table in our database file with three columns. An auto increment column called "ID", and two NVARCHAR type columns with the names "Key" and "Value"
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=ServerDB.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    try
                    {
                        con.Open();                             // Open the connection to the database
                        com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                        com.ExecuteNonQuery();
                        com.CommandText = "INSERT INTO ServerTable(ServerID, ServerURL) Values ('" + server.ServerID + "','" + server.ServerUrl + "')";
                        com.ExecuteNonQuery();      // Execute the query
                        con.Close();        // Close the connection to the database
                    }
                    catch 
                    {
                        throw new Exception("error in adding from server DB");
                    }
                }
            }
        }
        public Server GetServer(string ID)
        {
            //System.Data.SQLite.SQLiteConnection.CreateFile("databaseFile.db3");        // Create the file which will be hosting our database
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=ServerDB.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    Server server = new Server();
                    con.Open();
                    //com.CommandText = "Select * FROM MyTable WHEN ID =";

                    com.CommandText = "Select * FROM ServerTable WHERE ServerID = '" + ID + "'";      // Select all rows from our database table
                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                server.ServerID = reader["ServerID"].ToString();
                                server.ServerUrl = reader["ServerURL"].ToString();
                            }
                            catch
                            {
                                throw new Exception("error in reading the currect id from server DB");
                            }


                        }
                        reader.Close();
                    }

                    con.Close();
                    return server;
                }
            }
        }
        public void DeleteFromDB(string serverID)
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=ServerDB.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    try
                    {
                        con.Open();
                        string deleteString = @"delete from ServerTable where ServerID = '" + serverID + "'";
                        com.CommandText = deleteString;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                        con.Close();
                    }
                    catch
                    {
                        throw new Exception("error in deleting from server DB");
                    }
                }
            }
        }
        public List<Server> ReadFromDB()
        {
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=ServerDB.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    List<Server> listOfServer = new List<Server>();
                    try
                    {
                        con.Open();
                        com.CommandText = "Select * FROM ServerTable";      // Select all rows from our database table
                        using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Server server = new Server();

                                server.ServerID = reader["ServerID"].ToString();
                                server.ServerUrl = reader["ServerURL"].ToString();
                                listOfServer.Add(server);



                            }
                            reader.Close();

                        }
                    }
                    catch
                    {
                        throw new Exception("error in reading from server DB");
                    }
                    return listOfServer;
                }
            }
        }
    }
}