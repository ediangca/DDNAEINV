using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Views
{
    public class ItemVw
    {
        [Key]
        public required string  IID { get; set; }
        public required int IGID { get; set; }
        public required string ItemGroupName { get; set; }
        public required string Description { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}
