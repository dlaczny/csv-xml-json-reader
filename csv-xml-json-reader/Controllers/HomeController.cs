using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using csv_xml_json_reader.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using CsvHelper;
using csv_xml_json_reader.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

namespace csv_xml_json_reader.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;

        private readonly IOrderRepository _orderRepository;

        public List<string> expecion_string = new List<string>();

        public int index;

        //

        //

        public HomeController(AppDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        private IQueryable<Order> SortOrder(string sortOrder)
        {
            ViewData["clientIdSortParm"] = sortOrder == "clientId" ? "clientId_desc" : "clientId";
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";
            ViewData["nameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["quantitySortParm"] = sortOrder == "quantity" ? "quantity_desc" : "quantity";
            ViewData["priceSortParm"] = sortOrder == "price" ? "price_desc" : "price";

            var sortedOrders = from s in _context.Orders
                               select s;



            switch (sortOrder)
            {
                case "clientId":
                    sortedOrders = sortedOrders.OrderBy(s => s.clientId);
                    break;
                case "clientId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.clientId);
                    break;
                case "requestId":
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
                case "requestId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.requestId);
                    break;
                case "name":
                    sortedOrders = sortedOrders.OrderBy(s => s.name);
                    break;
                case "name_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.name);
                    break;
                case "quantity":
                    sortedOrders = sortedOrders.OrderBy(s => s.quantity);
                    break;
                case "quantity_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.quantity);
                    break;
                case "price":
                    sortedOrders = sortedOrders.OrderBy(s => s.price);
                    break;
                case "price_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.price);
                    break;
                default:
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
            }

            return sortedOrders;
        }
        private IEnumerable<Products> SortProduct(string sortOrder, string id = null)
        {
            ViewData["productNameSortParm"] = sortOrder == "ProductName_desc" ? "ProductName" : "ProductName_desc";
            ViewData["productCountSortParm"] = sortOrder == "ProductCount" ? "ProductCount_desc" : "ProductCount";

            var sortedOrders = from s in _orderRepository.ProductsCount(id)
                               select s;

            switch (sortOrder)
            {
                case "ProductName":
                    sortedOrders = sortedOrders.OrderBy(s => s.ProductName);
                    break;
                case "ProductName_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.ProductName);
                    break;
                case "ProductCount":
                    sortedOrders = sortedOrders.OrderBy(s => s.ProductCount);
                    break;
                case "ProductCount_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.ProductCount);
                    break;
                default:
                    sortedOrders = sortedOrders.OrderBy(s => s.ProductName);
                    break;
            }

            return sortedOrders;
        }

        public IActionResult Index(string sortOrder)
        {

            return (IActionResult)this.View((object)new VM(){});
        }

        //a. Ilość zamówień
        public IActionResult A()
        {
            return (IActionResult)this.View((object)new VM()
            {
                OrdersCount = _orderRepository.OrderModels().Count(),
            });
        }

        //b. Ilość zamówień dla klienta o wskazanym identyfikatorze
        public IActionResult B(string id)
        {
            return (IActionResult)this.View((object)new VM()
            {
                OrdersCount = _orderRepository.OrderModels().Where(i => i.clientId == id).Count()
            });
        }

        //c. Łączna kwota zamówień 
        public IActionResult C()
        {
            return (IActionResult)this.View((object)new VM()
            {
                TotalValueOfOrders = _orderRepository.TotalValueOfOrderss().ToString("c2")
            });
        }

        //d. Łączna kwota zamówień dla klienta o wskazanym identyfikatorze 
        public IActionResult D(string id)
        {
            return (IActionResult)this.View((object)new VM()
            {
                TotalValueOfOrders = _orderRepository.TotalValueOfOrderss(id).ToString("c2")
            });
        }

        //e. Lista wszystkich zamówień
        public IActionResult E(string sortOrder)
        {

            ViewData["clientIdSortParm"] = sortOrder == "clientId" ? "clientId_desc" : "clientId";
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";

            var sortedOrders = from s in _orderRepository.OrderModels()
                               select s;

            switch (sortOrder)
            {
                case "clientId":
                    sortedOrders = sortedOrders.OrderBy(s => s.clientId);
                    break;
                case "clientId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.clientId);
                    break;
                case "requestId":
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
                case "requestId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.requestId);
                    break;
                default:
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
            }

            return (IActionResult)this.View((object)new VM()
            {
                AllOrdersModel = sortedOrders
            });
        }

        public IActionResult Esave()
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "download/listoforders.csv";


            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(_orderRepository.OrderModels());
            }

            return RedirectToAction("E");
        }

        //f. Lista zamówień dla klienta o wskazanym identyfikatorze
        public IActionResult F(string sortOrder, string id)
        {
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";

            var sortedOrders = from s in _orderRepository.OrderModels()
                               select s;

            switch (sortOrder)
            {
                case "requestId":
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
                case "requestId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.requestId);
                    break;
                default:
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
            }

            return (IActionResult)this.View((object)new VM()
            {
                AllOrdersModel = sortedOrders
            });
        }

        public IActionResult Fsave(string clientId)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "download/listofordersid.csv";


            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(_orderRepository.OrderModels(clientId));
            }

            return RedirectToAction("F");
        }

        //g. Średnia wartość zamówienia
        public IActionResult G()
        {
            return (IActionResult)this.View((object)new VM()
            {
                AverageOrderValue = _orderRepository.Average().ToString("c2")

                //AverageOrderValue = _orderRepository.AverageOrderValuee().ToString("c2")
            });
        }

        //h. Średnia wartość zamówienia dla klienta o wskazanym identyfikatorze 
        public IActionResult H(string id)
        {
            return (IActionResult)this.View((object)new VM()
            {
                AverageOrderValue = _orderRepository.Average(id).ToString("c2")

                //AverageOrderValue = _orderRepository.AverageOrderValuee(id).ToString("c2")
            });
        }

        //i. Ilość zamówień pogrupowanych po nazwie
        public IActionResult I(string sortOrder)
        {
            IEnumerable<Products> sortedOrders = SortProduct(sortOrder);

            return (IActionResult)this.View((object)new VM()
            {
                ProductsCount = sortedOrders.ToList()
            });
        }

        //j. Ilość zamówień pogrupowanych po nazwie dla klienta o wskazanym identyfikatorze
        public IActionResult J(string sortOrder, string id)
        {
            IEnumerable<Products> sortedOrders = SortProduct(sortOrder,id);


            return (IActionResult)this.View((object)new VM()
            {
                ProductsCount = sortedOrders.ToList()
            });
        }

        //k. Zamówienia w podanym przedziale cenowym
        public IActionResult K(string sortOrder, string FromValue, string ToValue)
        {

            ViewData["clientIdSortParm"] = sortOrder == "clientId" ? "clientId_desc" : "clientId";
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";


                float a = float.Parse(FromValue, CultureInfo.InvariantCulture.NumberFormat);

                float b = float.Parse(ToValue, CultureInfo.InvariantCulture.NumberFormat);
  

            var sortedOrders = from s in _orderRepository.OrdersInPriceRange(a, b)
                               select s;

            switch (sortOrder)
            {
                case "clientId":
                    sortedOrders = sortedOrders.OrderBy(s => s.clientId);
                    break;
                case "clientId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.clientId);
                    break;
                case "requestId":
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
                case "requestId_desc":
                    sortedOrders = sortedOrders.OrderByDescending(s => s.requestId);
                    break;
                default:
                    sortedOrders = sortedOrders.OrderBy(s => s.requestId);
                    break;
            }

            return (IActionResult)View(new VM()
            {
                AllOrdersModel = sortedOrders,
                FromValue = FromValue,
                ToValue = ToValue
            });
        }




        public async Task<IActionResult> Post([Bind("Id,clientId,requestId,name,quantity,price")] Order order, List<IFormFile> files)
        {


            List<Order> Templist = new List<Order>();


            bool exception_bool = false;

            Order oneOrderEx = new Order();

            //
           


            foreach (var file in files)
            {

                    if (file.FileName.Contains(".json"))
                    {
                        index = 0;
                        var result = string.Empty;

                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            result = reader.ReadToEnd();
                        }

                        try
                        {
                            var json = JsonConvert.DeserializeObject<OrderList>(result);


                            foreach (var item in json.OrdersList.ToList())
                            {
                            JsonXml(ref exception_bool, ref oneOrderEx, file, item);
                            }
                        }
                        catch
                        {
                            exception_bool = true;
                            expecion_string.Add("Błąd w pliku " + file.FileName);
                        }
                        
                    }

                    if (file.FileName.Contains(".xml"))
                    {
                        index = 0;
                        var result = string.Empty;

                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            result = reader.ReadToEnd();
                        }

                        
                        try
                        {
                                XmlSerializer serializer = new XmlSerializer(typeof(List<request>), new XmlRootAttribute("requests"));

                                StringReader stringReader = new StringReader(result);

                                List<request> productList = (List<request>)serializer.Deserialize(stringReader);

                                List<Order> orders = new List<Order>();

                                foreach (var item in productList)
                                {
                                    orders.Add(new Order {
                                        clientId = item.clientId,
                                        requestId = item.requestId,
                                        name = item.name,
                                        quantity = item.quantity,
                                        price = item.price
                                    });
                                }

                                foreach (var item in orders)
                                {
                                    JsonXml(ref exception_bool, ref oneOrderEx, file, item);
                                }
                                await _context.SaveChangesAsync();
                            }
                        catch
                        {
                            exception_bool = true;
                            expecion_string.Add("Błąd w pliku " + file.FileName);
                        }
            }

                if (file.FileName.Contains(".csv"))
                {

                    var regClientId = new Regex(@"^(?=.{1,6}$)^[a-zA-Z0-9]*$");
                    var regName = new Regex(@"^(?=.{1,255}$)");



                    try
                    {
                        index = 0;
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        using (var csv = new CsvReader(reader))
                        {
                            OrderNoId orderNoId = new OrderNoId();

                            var records = csv.EnumerateRecords(orderNoId);

                            foreach (var item in records)
                            {

                                index++;

                                if (regClientId.IsMatch(item.Client_Id) && item.Request_id != 0 && regName.IsMatch(item.Name))
                                {
                                    Order oneOrder = new Order();

                                    oneOrder.clientId = item.Client_Id;
                                    oneOrder.requestId = item.Request_id;
                                    oneOrder.quantity = item.Quantity;
                                    oneOrder.price = item.Price;
                                    oneOrder.name = item.Name;

                                    _context.Add(oneOrder);

                                    _context.SaveChanges();
                                }
                                else
                                {
                                    exception_bool = true;
                                    expecion_string.Add("Błąd w pliku " + file.FileName + " w rekordzie " + index.ToString());

                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        exception_bool = true;
                        expecion_string.Add("Błąd w pliku " + file.FileName);
                    }

                    
                }
            }

            if (exception_bool)
            {
                return RedirectToAction("Error", new { error_string = expecion_string });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        private void JsonXml(ref bool exception_bool, ref Order oneOrderEx, IFormFile file, Order item)
        {

            var regClientId = new Regex(@"^(?=.{1,6}$)^[a-zA-Z0-9]*$");
            var regName = new Regex(@"^(?=.{1,255}$)");

            index++;

            if (regClientId.IsMatch(item.clientId) && item.requestId != 0 && regName.IsMatch(item.name))
            {
                Order oneOrder = new Order();

                oneOrder.clientId = item.clientId;
                oneOrder.requestId = item.requestId;
                oneOrder.quantity = item.quantity;
                oneOrder.price = item.price;
                oneOrder.name = item.name;

                _context.Add(oneOrder);

                _context.SaveChanges();
            }
            else
            {
                exception_bool = true;
                expecion_string.Add("Błąd w pliku " + file.FileName + " w rekordzie " + index.ToString());

            }
        }

        public IActionResult Error(List<string> error_string)
        {
            return View(new VM()
            {
                Error = error_string
            });
        }
    }
}
