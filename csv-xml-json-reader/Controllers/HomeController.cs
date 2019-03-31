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

        //zmienne do listowania błędów
        public List<string> expecion_string = new List<string>();
        public int indexOfOrder;
        //

        public HomeController(AppDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        public IActionResult Index(string sortOrder)
        {
            return (IActionResult)this.View((object)new VM() { });
        }

        #region Sortowanie
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
        #endregion

        #region Generowanie Raportów

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

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            List<OrderNoId> orderNoIds = new List<OrderNoId>();

            foreach (var item in _context.Orders)
            {
                OrderNoId orderNoId = new OrderNoId();

                orderNoId.Client_Id = item.clientId;
                orderNoId.Request_id = item.requestId;
                orderNoId.Name = item.name;
                orderNoId.Price = item.price;
                orderNoId.Quantity = item.quantity;

                orderNoIds.Add(orderNoId);
            }

            int version = 0;

            string file = path + "/listoforders.csv";

            while (System.IO.File.Exists(file))
            {
                version++;

                file = path + "/listoforders(" + version.ToString() + ").csv";
            }


            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(orderNoIds);
            }

            return RedirectToAction("Index");
        }

        //f. Lista zamówień dla klienta o wskazanym identyfikatorze
        public IActionResult F(string sortOrder, string id)
        {
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";

            var sortedOrders = from s in _orderRepository.OrderModels(id)
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


            //string path = AppDomain.CurrentDomain.BaseDirectory + "download/listofordersid.csv";

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


            List<OrderNoId> orderNoIds = new List<OrderNoId>();

            foreach (var item in _context.Orders.ToList().Where(i => i.clientId == clientId))
            {
                OrderNoId orderNoId = new OrderNoId();

                orderNoId.Client_Id = item.clientId;
                orderNoId.Request_id = item.requestId;
                orderNoId.Name = item.name;
                orderNoId.Price = item.price;
                orderNoId.Quantity = item.quantity;

                orderNoIds.Add(orderNoId);
            }

            int version = 1;

            string file = path + "/listoforders-" + clientId + ".csv";

            while (System.IO.File.Exists(file))
            {
                version++;

                file = path + "/listoforders-" + clientId + "(" + version.ToString() + ").csv";
            }

            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer))
            {
                //csv.WriteRecords(_context.Orders.ToList().Where(i => i.clientId == clientId));
                csv.WriteRecords(orderNoIds);

            }

            return RedirectToAction("Index");
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

        public IActionResult Isave()
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


            var products = from s in _orderRepository.ProductsCount()
                               select s;


            int version = 1;

            string file = path + "/listofproducts.csv";

            while (System.IO.File.Exists(file))
            {
                version++;

                file = path + "/listofproducts(" + version.ToString() + ").csv";
            }

            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer))
            {
                //csv.WriteRecords(_context.Orders.ToList().Where(i => i.clientId == clientId));
                csv.WriteRecords(products);

            }
            return RedirectToAction("Index");
        }

        //j. Ilość zamówień pogrupowanych po nazwie dla klienta o wskazanym identyfikatorze
        public IActionResult J(string sortOrder, string id)
        {
            IEnumerable<Products> sortedOrders = SortProduct(sortOrder,id);


            return (IActionResult)this.View((object)new VM()
            {
                ProductsCount = sortedOrders.ToList(),
                id = id
            });
        }

        public IActionResult Jsave(string id)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


            var products = from s in _orderRepository.ProductsCount(id)
                           select s;


            int version = 1;

            string file = path + "/listofproducts-id-" + id +".csv";

            while (System.IO.File.Exists(file))
            {
                version++;

                file = path + "/listofproducts-id-" + id + "(" + version.ToString() + ").csv";
            }

            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer))
            {
                //csv.WriteRecords(_context.Orders.ToList().Where(i => i.clientId == clientId));
                csv.WriteRecords(products);

            }
            return RedirectToAction("Index");
        }

        //k. Zamówienia w podanym przedziale cenowym
        public IActionResult K(string sortOrder, string FromValue, string ToValue)
        {

            ViewData["clientIdSortParm"] = sortOrder == "clientId" ? "clientId_desc" : "clientId";
            ViewData["requestIdSortParm"] = sortOrder == "requestId" ? "requestId_desc" : "requestId";

            try
            {
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
            catch
            {
                return RedirectToAction("Error", new { error_string = "Błędny zakres" });
            }
        }

        public IActionResult Ksave(string FromValue, string ToValue)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            float a = float.Parse(FromValue, CultureInfo.InvariantCulture.NumberFormat);

            float b = float.Parse(ToValue, CultureInfo.InvariantCulture.NumberFormat);


            var sortedOrders = from s in _orderRepository.OrdersInPriceRange(a, b)
                               select s;


            int version = 1;

            string file = path + "/listofordersbetween.csv";

            while (System.IO.File.Exists(file))
            {
                version++;

                file = path + "/listofordersbetween(" + version.ToString() + ").csv";
            }

            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer))
            {
                //csv.WriteRecords(_context.Orders.ToList().Where(i => i.clientId == clientId));
                csv.WriteRecords(sortedOrders);

            }
            return RedirectToAction("Index");
        }

        #endregion


        #region Dodawanie plików
        public async Task<IActionResult> Post([Bind("Id,clientId,requestId,name,quantity,price")] Order order, List<IFormFile> files)
        {


            List<Order> Templist = new List<Order>();


            bool exception_bool = false;

            Order oneOrderEx = new Order();


            foreach (var file in files)
            {

                    if (file.FileName.Contains(".json"))
                    {
                        indexOfOrder = 0;
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
                        indexOfOrder = 0;
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
                        indexOfOrder = 0;
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        using (var csv = new CsvReader(reader))
                        {
                            OrderNoId orderNoId = new OrderNoId();

                            var records = csv.EnumerateRecords(orderNoId);

                            foreach (var item in records)
                            {

                                indexOfOrder++;

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
                                    expecion_string.Add("Błąd w pliku " + file.FileName + " w rekordzie " + indexOfOrder.ToString());

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

            indexOfOrder++;

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
                expecion_string.Add("Błąd w pliku " + file.FileName + " w rekordzie " + indexOfOrder.ToString());

            }
        }

        #endregion

        public IActionResult Error(List<string> error_string)
        {
            return View(new VM()
            {
                Error = error_string
            });
        }
    }
}
