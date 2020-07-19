using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IProductModel<T>
    {
        IEnumerable<T> GetAllProduct();
        T GetProductById(string id);
        void Add(T product);
        void Delete(string id);
    }
}
