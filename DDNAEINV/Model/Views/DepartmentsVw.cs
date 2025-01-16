namespace DDNAEINV.Model.Views
{
    public class DepartmentsVw
    {
        public int DepID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public required int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int NoOfSection { get; set; } = 0;
        public DateTime Date_Created { get; set; }
        public DateTime Date_Updated { get; set; }
    }
}
