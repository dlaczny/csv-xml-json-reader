using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace csv_xml_json_reader.Models
{
    //W jaki sposób podanie są zamówienia w plikach
    public class Order
    {

        [Key]
        public int Id { get; set; }

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
