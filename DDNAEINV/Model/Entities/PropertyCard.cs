using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Entities
{
    public class PropertyCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? PCNo { get; set; } = null;
        public string? Ref { get; set; }= string.Empty;
        public string? REFNoFrom { get; set; } = string.Empty;
        public string? REFNoTo { get; set; } = string.Empty;
        public int? itemNo { get; set; } = null;
        public string? propertyNo { get; set; } = string.Empty;
        public string? issuedBy { get; set; } = string.Empty;
        public string? receivedBy { get; set; } = string.Empty;
        public string? approvedBy { get; set; } = string.Empty;
        public string? createdBy { get; set; } = string.Empty;

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Created { get; set; } = DateTime.Now;

    }
}
