using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
using FlightControlWeb.DB;
using System.Net.Http;
using System.Net;
using System.IO;
using FlightControlWeb.middle;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private IProductModel<FlightPlan> Iflight;
        private IProductModel<Server> Iserver;

        public FlightPlanController(IProductModel<FlightPlan> Iflight, IProductModel<Server> Iserver)
        {
            this.Iflight = Iflight;
            this.Iserver = Iserver;
        }
        // GET: api/FlightPlan
        [HttpGet("{id}")]
        public IActionResult getFlightPlan(string id)
        {
            try
            {
                return Ok(Iflight.GetProductById(id));
            }
            catch
            {
                List<Server> listOfServer = Iserver.GetAllProduct().ToList();
                HttpClient httpClient = new HttpClient();
                foreach (Server s in listOfServer)
                {
                    FlightPlanMiddle planMiddle = new FlightPlanMiddle();
                    FlightPlan flightPlan = planMiddle.getFlightsFromServer(s, id);
                    return Ok(flightPlan);
                }
                return BadRequest();
            }
        }

        // POST: api/FlightPlan
        [HttpPost]
        public IActionResult POST([FromBody] FlightPlan flightPlan)
        {
            try
            {
                Iflight.Add(flightPlan);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}