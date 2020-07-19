using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.DB;

namespace FlightControlWeb.Models
{
    public class FlightModel : IProductModel<FlightPlan>
    {
        private FlightsDataBase DB;
        public FlightModel()
        {
            DB = new FlightsDataBase();
        }
        public FlightsDataBase getDB()
        {
            return this.DB;
        }
        public void Add(FlightPlan _flight)
        {

            DB.AddTODB(_flight);
        }

        public void Delete(string id)
        {
            DB.DeleteFromDB(id);
        }

        public IEnumerable<FlightPlan> GetAllProduct()
        {
            try
            {
                return DB.ReadFromDB();
            }
            catch
            {
                return new List<FlightPlan>().ToList();
            }
        }
        public FlightPlan GetProductById(string id)
        {
            return DB.GetFlightPlan(id);
        }
    }
}
