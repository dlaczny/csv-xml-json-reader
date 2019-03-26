using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace csv_xml_json_reader.Models
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;

        public float totalValue;

        public float totalValue2;

        public OrderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<OrderModel> OrderModels(string id = null)
        {
            List<OrderModel> list = OrderModelsMethod();
            return list;
        }

        public List<Products> ProductsCount(string id = null)
        {
            List<Products> ListOfNames = new List<Products>();


            if (String.IsNullOrEmpty(id))
            {
                foreach (var item in _appDbContext.Orders.ToList())
                {
                    if (!ListOfNames.Any(a => a.ProductName == item.name))
                    {
                        ListOfNames.Add(new Products()
                        {
                            ProductName = item.name
                        });
                    }
                    foreach (var item2 in ListOfNames.Where(p => p.ProductName == item.name))
                    {
                        item2.ProductCount++;
                    }
                }
            }
            else
            {
                foreach (var item in _appDbContext.Orders.Where(i => i.clientId == id).ToList())
                {
                    if (!ListOfNames.Any(a => a.ProductName == item.name))
                    {
                        ListOfNames.Add(new Products()
                        {
                            ProductName = item.name
                        });
                    }
                    foreach (var item2 in ListOfNames.Where(p => p.ProductName == item.name))
                    {
                        item2.ProductCount++;
                    }
                }
            }

               

            return ListOfNames;
        }

        private List<OrderModel> OrderModelsMethod()
        {
            List<OrderModel> list = new List<OrderModel>();

            foreach (var item in _appDbContext.Orders)
            {
                if (list.Any(a => a.requestId == item.requestId) && list.Any(b => b.clientId == item.clientId))
                {
                    list.Find(a => (a.requestId == item.requestId) && (a.clientId == item.clientId)).OrderModelDetails.Add(new OrderModelDetails()
                    {

                        name = item.name,
                        price = item.price,
                        quantity = item.quantity
                    });
                }
                else
                {
                    List<OrderModelDetails> list2 = new List<OrderModelDetails>();

                    list2.Add(new OrderModelDetails() { name = item.name, price = item.price, quantity = item.quantity });

                    list.Add(new OrderModel()
                    {

                        clientId = item.clientId,
                        requestId = item.requestId,
                        OrderModelDetails = list2
                    });
                }
            }

            return list;
        }

        public List<OrderModel> OrdersInPriceRange(float a, float b)
        {
            List<OrderModel> list = OrderModelsMethod();

            foreach (var item2 in list.ToList())
            {

                foreach (var item in item2.OrderModelDetails.ToList())
                {

                    if (a < (float)item.quantity * item.price && b > (float)item.quantity * item.price)
                    {
                        if(!list.Any(x => x == item2))
                        {
                            list.Add(item2);
                        }
                        
                    }
                }

            }

            return list;
        }

        public float TotalValueOfOrderss(string id = null)
        {
            if (String.IsNullOrEmpty(id))
            {
                foreach (var item in _appDbContext.Orders.ToList())
                {
                    totalValue += (float)item.quantity * item.price;
                }
            }
            else
            {
                foreach (var item in _appDbContext.Orders.Where(i => i.clientId == id).ToList())
                {
                    totalValue += (float)item.quantity * item.price;
                }
            }
           

            return totalValue;
        }

        public float Average(string id = null)
        {
            List<OrderModel> list = OrderModelsMethod();

            float value;

            if (String.IsNullOrEmpty(id))
            {
                foreach (var item2 in list)
                {
                    foreach (var item3 in item2.OrderModelDetails.ToList())
                    {
                        totalValue2 += (float)item3.quantity * item3.price;
                    }
                }
            }
            else
            {
                foreach (var item2 in list.Where(i => i.clientId == id))
                {
                    foreach (var item3 in item2.OrderModelDetails.ToList())
                    {
                        totalValue2 += (float)item3.quantity * item3.price;
                    }
                }
            } 

            value = totalValue2 / list.Count();

            return value;
        }
    }
}
