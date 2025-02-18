namespace DDNAEINV.Model.Views
{
    public class PropertyDetailsVw
    {
        public required int ID {get; set; }
        public string? REF { get; set; } = String.Empty;
        public string? REFNo { get; set; } = String.Empty;
        public string? Item { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? SerialNo { get; set; } = String.Empty;
        public string? PropertyNo { get; set; } = String.Empty;
        public string? QRCode { get; set; } = String.Empty;
        public double? amount { get; set; } = 0.0;
        public DateTime Date_Acquired { get; set; }
    }
}
