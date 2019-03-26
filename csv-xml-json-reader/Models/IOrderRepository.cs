using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csv_xml_json_reader.Models
{
    public interface IOrderRepository
    {
        float TotalValueOfOrderss(string id = null);

        float Average(string id = null);

        List<OrderModel> OrdersInPriceRange(float a, float b);

        List<OrderModel> OrderModels(string id = null);

        List<Products> ProductsCount(string id = null);
    }
}
