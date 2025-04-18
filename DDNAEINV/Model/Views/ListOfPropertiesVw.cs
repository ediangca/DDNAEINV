﻿namespace DDNAEINV.Model.Views
{
    public class ListOfPropertiesVw
    {
        public string Module { get; set; } = string.Empty; 
        public int? ItemNo { get; set; } = 0;
        public string? RefNo { get; set; } = String.Empty;
        public string? IID { get; set; } = String.Empty;
        public string? Brand { get; set; } = String.Empty;
        public string? Model { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? SerialNo { get; set; } = String.Empty;
        public string? PropertyNo { get; set; } = String.Empty;
        public string? QRCode { get; set; } = String.Empty;
        public string? Unit { get; set; } = String.Empty;
        public decimal? Amount { get; set; } = 0;
        public DateTime? Date_Acquired { get; set; }
        public decimal? EUL { get; set; } = 0;
        public bool? TransferFlag { get; set; } = false;
        public string? TransferType { get; set; } = String.Empty;
        public string? TransferOthersType { get; set; } = String.Empty;
        public string? TransferReason { get; set; } = String.Empty;
        public string? TransferNo { get; set; } = String.Empty;
        public bool? ReturnFlag { get; set; } = false;
        public string? ReturnType { get; set; } = String.Empty;
        public string? ReturnOthersType { get; set; } = String.Empty;
        public string? ReturnNo { get; set; } = String.Empty;
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
        public DateTime? DateCreated { get; set; }
    }
}
