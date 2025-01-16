using DDNAEINV.Model.Entities;

namespace DDNAEINV.Model.Views
{
    public class BranchesVw
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int NoOfDepartment { get; set; } = 0;
        public DateTime Date_Created { get; set; }
        public DateTime Date_Updated { get; set; }
    }
}
