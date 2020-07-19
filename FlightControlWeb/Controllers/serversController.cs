using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;
namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class serversController : ControllerBase
    {
        private IProductModel<Server> Iserver;
        public serversController(IProductModel<Server> Iserver)
        {
            this.Iserver = Iserver;
        }

        [HttpGet]
        public Respone<List<Server>> GetAllServers()
        {
            Respone<List<Server>> respone = new Respone<List<Server>>();
            try
            {
                List<Server> listOfServer = Iserver.GetAllProduct().ToList();
                return respone.ResponeSuccess(listOfServer);
            }
            catch
            {
                return respone.error300(null);
            }
        }
        [HttpPost]
        public IActionResult AddServer([FromBody] Server server)
        {
            try
            {
                Iserver.Add(server);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult deleteServer(string id)
        {
            try
            {
                Iserver.Delete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

    }
}