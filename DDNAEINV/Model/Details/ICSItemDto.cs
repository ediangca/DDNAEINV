using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Details
{
    public class ICSItemDto
    {
        public int? ICSItemNo { get; set; } = 0;
        public string? ITRNo { get; set; } = string.Empty;
        public required string ICSNo { get; set; }
        public required string IID { get; set; }
        public required string Description { get; set; }
        public string? SerialNo { get; set; } = string.Empty;
        public string? PropertyNo { get; set; } = string.Empty;
        public string? QRCode { get; set; } = string.Empty;
        public required string Unit { get; set; }
        public required decimal Amount { get; set; }
        public required int Qty { get; set; }
        public required decimal EUL { get; set; }

    }
}
