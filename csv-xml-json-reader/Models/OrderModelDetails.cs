using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace csv_xml_json_reader.Models
{
    //Detale dla pojedyńczego zamówienia
    public class OrderModelDetails
    {

        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Too long name")]
        public string name { get; set; }

        [Required]
        public int quantity { get; set; }

        // numeryczne zmiennoprzecinkowe podwójnej precyzji
        [Required]
        [Column(TypeName = "money")]
        public float price { get; set; }
    }
}
