using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlightControlWeb;
using FlightControlWeb.Controllers;
using FlightControlWeb.middle;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private IProductModel<FlightPlan> Iflight;
        private IProductModel<Server> Iserver;

        public FlightsController(IProductModel<FlightPlan> Iflight, IProductModel<Server> _Iserver)
        {
            this.Iflight = Iflight;
            this.Iserver = _Iserver;
        }
        [HttpGet]
        public Respone<List<Flight>> GetAllFlightsByRelativeTo([FromQuery] string relative_to)
        {
            Respone<List<Flight>> respone = new Respone<List<Flight>>();
            string s = Request.QueryString.Value;
            List<Flight> listFlights = new List<Flight>();
            FlightMiddle flightMiddle = new FlightMiddle();
            Flight flight = new Flight();

            if (s.Contains("sync_all"))
            {
                try
                {
                    listFlights = flightMiddle.GetListFlightFromServer(Iserver, Iflight, relative_to);
                    return respone.ResponeSuccess(listFlights);
                }
                catch
                {
                    return respone.error500(null);
                }
            }
            else
            {
                List<FlightPlan> listFromcurrentServer = Iflight.GetAllProduct().ToList();
                try
                {
                    listFlights = flight.fromPlanToFlight((FlightModel)Iflight, Iflight.GetAllProduct().ToList(), relative_to, false);
                }
                catch
                {
                    return respone.error500(null);
                }
                if (listFlights.Count == 0)
                {
                    return respone.error300(null);
                }
                else
                {
                    respone.ErrorId = 200;
                }
                return respone.ResponeSuccess(listFlights);

            }
        }

        // DELETE api/Flight/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                Iflight.Delete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}