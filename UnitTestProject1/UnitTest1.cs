using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb;
using FlightControlWeb.Models;
using FlightControlWeb.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading;
using FlightControlWeb.DB;
using FlightControl.Controllers;
using System;

namespace FlightControlWebTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Delete_flightController_deleteFlightFromsDB()
        {
            var mockFlightPlan = new Mock<IProductModel<FlightPlan>>();
            var mockServer = new Mock<IProductModel<Server>>();
            var controller = new FlightsController(mockFlightPlan.Object, mockServer.Object);

            // Act
            IActionResult actionResult = controller.Delete("");

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

         [TestMethod]
        public void DeleteServer_ServerController_deleteServerFromsDB()
        {
            var mockServer = new Mock<IProductModel<Server>>();
            var controller = new serversController(mockServer.Object);

            // Act
            IActionResult actionResult = controller.deleteServer("");

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public void AddToDB()
        {
            var mockFlightPlan = new Mock<IProductModel<FlightPlan>>();
            FlightPlan f1 = new FlightPlan();

              // FlightID = "london",
              f1.Company_Name = "ofirTours";
              f1.Passengers = 10;
            f1.Initial_location = new Initial_location();
            f1.Initial_location.Longitude = 32.00000;
            f1.Initial_location.date_time = "2020-13-05T23:56:00Z";
            f1.Initial_location.Latitude = 32.00000;
            f1.Segments = new List<Segment>();
              Segment s = new Segment();
              s.Latitude = 32.00005;
              s.Latitude = 32.00005;
              s.timespan_seconds = 650;
              f1.Segments.Add(s);
            // var mockDB = new Mock<FlightPlan>(f1);
            // var db = FlightsDataBase(mockDB);

            mockFlightPlan.Setup(m => m.Add(f1));
     
        }

        [TestMethod]
        public void DeletAndAddFlightTODB()
        {
            FlightPlan f1 = new FlightPlan();

            // FlightID = "london",
            f1.Company_Name = "ofirTours";
            f1.Passengers = 10;
            f1.Initial_location = new Initial_location();
            f1.Initial_location.Longitude = 32.00000;
            f1.Initial_location.date_time = "2020-13-05T23:56:00Z";
            f1.Initial_location.Latitude = 32.00000;
            f1.Segments = new List<Segment>();
            Segment s = new Segment();
            s.Latitude = 32.00005;
            s.Latitude = 32.00005;
            s.timespan_seconds = 650;
            f1.Segments.Add(s);

            FlightsDataBase flightsDataBase = new FlightsDataBase();
            flightsDataBase.AddTODB(f1);
            List<FlightPlan> listS1 = flightsDataBase.ReadFromDB();
            if(listS1.Count != 1)
            {
                throw new Exception();
            }
            flightsDataBase.DeleteFromDB(f1.getFlightPlanID());
            listS1= flightsDataBase.ReadFromDB();
            if (listS1.Count != 0)
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void AddAndDeletServer()
        {
            Server s = new Server
            {
                ServerID = "serverTest",
                ServerUrl = "url/test"
            };
            ServerDataBase s_db = new ServerDataBase();
            s_db.AddTODB(s);
            List<Server> listS1 = s_db.ReadFromDB();
            if (listS1.Count != 1)
            {
                throw new Exception();
            }
            s_db.DeleteFromDB("serverTest");
            listS1 = s_db.ReadFromDB();
            if (listS1.Count != 0)
            {
                throw new Exception();
            }
        }
    }
}
