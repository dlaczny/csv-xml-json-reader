using csv_xml_json_reader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csv_xml_json_reader.ViewModels
{
    public class VM
    {
        public int OrdersCount { get; set; }

        public string TotalValueOfOrders { get; set; }

        public string AverageOrderValue { get; set; }


        public List<OrderModel> OrdersInPriceRangee { get; set; }

        public IEnumerable<Order> AllOrders { get; set; }

        public IEnumerable<OrderModel> AllOrdersModel { get; set; }


        public List<string> Error { get; set; }

        public List<Products> ProductsCount { get; set; }



        //

        public string FromValue { get; set; }

        public string ToValue { get; set; }

    }
}
