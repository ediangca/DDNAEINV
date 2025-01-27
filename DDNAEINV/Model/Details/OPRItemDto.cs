using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Entities
{
    public class OPRItemDto
    {
        public int? OPRINO { get; set; } = 0;
        public required int oprNo { get; set; }
        public required string IID { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string Description { get; set; }
        public required string SerialNo { get; set; }
        public required string PropertyNo { get; set; }
        public required string QRCode { get; set; }
        public required string Unit { get; set; }
        public required double Amount { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Acquired { get; set; } = DateTime.Now;

    }
}
