using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Details
{
    public class ParItemDto
    {
        public int? PARINO { get; set; } = 0;
        public required string PARNo { get; set; }
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
        public required DateTime Date_Acquired { get; set; }

    }
}
