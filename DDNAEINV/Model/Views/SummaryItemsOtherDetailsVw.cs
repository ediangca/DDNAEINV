namespace DDNAEINV.Model.Views
{
    public class SummaryItemsOtherDetailsVw
    {
        public int? OPRINO { get; set; } = 0;
        public int? OPRNo { get; set; } = 0;
        public string? IID { get; set; } = String.Empty;
        public string? Brand { get; set; } = String.Empty;
        public string? Model { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? SerialNo { get; set; } = String.Empty;
        public string? PropertyNo { get; set; } = String.Empty;
        public string? QRCode { get; set; } = String.Empty;
        public string? Unit { get; set; } = String.Empty;
        public double? Amount { get; set; } = 0.0;
        public string? Module { get; set; } = String.Empty;
        public bool? postFlag { get; set; } = false;
        public string? TransferType { get; set; } = String.Empty;
        public string? TransferOthersType { get; set; } = String.Empty;
        public string? TransferReason { get; set; } = String.Empty;
        public string? ReturnType { get; set; } = String.Empty;
        public string? ReturnOthersType { get; set; } = String.Empty;
        public string? Issued { get; set; } = String.Empty;
        public string? IssuedBy { get; set; } = String.Empty;
        public string? IssuedByOffice { get; set; } = String.Empty;
        public string? Received { get; set; } = String.Empty;
        public string? ReceivedBy { get; set; } = String.Empty;
        public string? ReceivedByOffice { get; set; } = String.Empty;
        public string? Approved { get; set; } = String.Empty;
        public string? ApprovedBy { get; set; } = String.Empty;
        public string? ApprovedByOffice { get; set; } = String.Empty;
        public string? Created { get; set; } = String.Empty;
        public string? CreatedBy { get; set; } = String.Empty;
        public string? CreatedByOffice { get; set; } = String.Empty;
        public DateTime Date_Acquired { get; set; }
        public bool? optrFlag { get; set; } = false;
        public string? OPTRNo { get; set; } = String.Empty;
        public bool? oprrFlag { get; set; } = false;
        public string? OPRRNo { get; set; } = String.Empty;
    }
}
