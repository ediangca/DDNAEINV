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
        public string? REF { get; set; }= string.Empty;
        public string? REFNoFrom { get; set; } = string.Empty;
        public string? REFNoTo { get; set; } = string.Empty;
        public string? PropertyNo { get; set; } = string.Empty;
        public string? IssuedBy { get; set; } = string.Empty;
        public string? ReceivedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; } = string.Empty;
        public string? CreatedBy { get; set; } = string.Empty;

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Date_Created { get; set; } = DateTime.Now;

    }
}
