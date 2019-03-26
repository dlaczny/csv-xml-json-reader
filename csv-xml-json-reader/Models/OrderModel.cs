using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace csv_xml_json_reader.Models
{
    public class OrderModel
    {
        [Key]
        public int id { get; set; }

        //max 6 alfanumerycznie
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$",
            ErrorMessage = "Tylko znaki alfanumerycznie bez spacji")]
        [StringLength(6, ErrorMessage = "Too long clientId")]
        public string clientId { get; set; }

        //long
        [Required]
        public long requestId { get; set; }

        [Required]
        public List<OrderModelDetails> OrderModelDetails { get; set; }
    }
}
