using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Entities
{
    public class RRSEP
    {
        [Key]
        public required string RRSEPNo { get; set; }
        public string? entityName { get; set; } = String.Empty;
        public string? rtype { get; set; } = String.Empty;
        public string? otype { get; set; } = String.Empty;
        public string? issuedBy { get; set; } = String.Empty;
        public string? receivedBy { get; set; } = String.Empty;
        public string? approvedBy { get; set; } = String.Empty;
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Created { get; set; } = DateTime.Now;

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Updated { get; set; } = null;

    }
}
