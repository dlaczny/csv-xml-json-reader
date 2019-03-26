using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csv_xml_json_reader.Models
{
    public class OrderNoId
    {
      
        
        //max 6 alfanumerycznie
        public string Client_Id { get; set; }

        //long
        public long Request_id { get; set; }

        //[Column(TypeName = "varchar(255)")]
        public string Name { get; set; }

        public int Quantity { get; set; }

        // numeryczne zmiennoprzecinkowe podwójnej precyzji 
        public float Price { get; set; }
    }
}

