using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.DB;
namespace FlightControlWeb.Models
{
    public class ServerModel : IProductModel<Server>
    {
        private ServerDataBase DB = new ServerDataBase();

        public void Add(Server server)
        {
            DB.AddTODB(server);
        }

        public void Delete(string id)
        {
            DB.DeleteFromDB(id);
        }

        public IEnumerable<Server> GetAllProduct()
        {
            try
            {
                return DB.ReadFromDB();
            }
            catch
            {
                return new List<Server>();
            }
        }

        public Server GetProductById(string id)
        {
            return DB.GetServer(id);
        }

    }
}
