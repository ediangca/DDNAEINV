using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Entities
{
    public class ICSItemVw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ICSItemNo { get; set; }
        public required string ICSNo { get; set; }
        public required string IID { get; set; }
        public string? Brand { get; set; } = string.Empty;
        public string? Model { get; set; } = string.Empty;
        public required string Description { get; set; }
        public string? SerialNo { get; set; } = string.Empty;
        public string? PropertyNo { get; set; } = string.Empty;
        public string? QRCode { get; set; } = string.Empty;
        public required string Unit { get; set; }
        public required decimal Amount { get; set; }
        public required int Qty { get; set; }
        public required decimal EUL { get; set; }
        public bool? itrFlag { get; set; } = false;
        public string? ITRNo { get; set; }
        public bool? rrsepFlag { get; set; } = false;
        public string? RRSEPNo { get; set; } = string.Empty;

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Acquired { get; set; } = DateTime.Now;


    }
}
