namespace DDNAEINV.Model.Views
{
    public class PropertyCardDetailsVw
    {
        public required int PCNo {get; set; }
        public string? REF { get; set; } = String.Empty;
        public string? RefNoFrom { get; set; } = String.Empty;
        public string? RefNoTo { get; set; } = String.Empty;
        public string? PropertyNo { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? QRCode { get; set; } = String.Empty;
        public string? IssuedBy { get; set; } = String.Empty;
        public string? Issued { get; set; } = String.Empty;
        public string? Received { get; set; } = String.Empty;
        public string? ReceivedBy { get; set; } = String.Empty;
        public string? ApprovedBy { get; set; } = String.Empty;
        public string? Approved { get; set; } = String.Empty;
        public string? CreatedBy { get; set; } = String.Empty;
        public string? Created { get; set; } = String.Empty;
        public DateTime Date_Created { get; set; }
    }
}
