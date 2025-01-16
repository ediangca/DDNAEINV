namespace DDNAEINV.Model.Views
{
    public class PRSItemsDetailsVw
    {
        public required int PARINO { get; set; } = 0;
        public required string PARNo { get; set; } = String.Empty;
        public required string IID { get; set; } = String.Empty;
        public string? Brand { get; set; } = String.Empty;
        public string? Model { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? SerialNo { get; set; } = String.Empty;
        public required string PropertyNo { get; set; }
        public required string QRCode { get; set; }
        public required string Unit { get; set; }
        public required double Amount { get; set; }
        public required string ReturnType { get; set; }
        public required string ReturnOthersType { get; set; }
        public string? Issued { get; set; } = String.Empty;
        public string? IssuedBy { get; set; } = String.Empty;
        public string? IssuedByBranch { get; set; } = String.Empty;
        public string? IssuedByDepartment { get; set; } = String.Empty;
        public string? IssuedBySection { get; set; } = String.Empty;
        public string? IssuedByOffice { get; set; } = String.Empty;
        public string? Received { get; set; } = String.Empty;
        public string? ReceivedBy { get; set; } = String.Empty;
        public string? ReceivedByBranch { get; set; } = String.Empty;
        public string? ReceivedByDepartment { get; set; } = String.Empty;
        public string? ReceivedBySection { get; set; } = String.Empty;
        public string? ReceivedByOffice { get; set; } = String.Empty;
        public string? Approved { get; set; } = String.Empty;
        public string? ApprovedBy { get; set; } = String.Empty;
        public string? ApprovedByBranch { get; set; } = String.Empty;
        public string? ApprovedByDepartment { get; set; } = String.Empty;
        public string? ApprovedBySection { get; set; } = String.Empty;
        public string? ApprovedByOffice { get; set; } = String.Empty;
        public string? Created { get; set; } = String.Empty;
        public string? CreatedBy { get; set; } = String.Empty;
        public string? CreatedByBranch { get; set; } = String.Empty;
        public string? CreatedByDepartment { get; set; } = String.Empty;
        public string? CreatedBySection { get; set; } = String.Empty;
        public string? CreatedByOffice { get; set; } = String.Empty;
        public DateTime Date_Acquired { get; set; }
        public required bool ReparFlag { get; set; }
        public string? REPARNo { get; set; } = String.Empty;
        public required bool PrsFlag { get; set; }
        public string? PRSNo { get; set; } = String.Empty;
    }
}
