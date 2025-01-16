using DDNAEINV.Model.Entities;

namespace DDNAEINV.Model.Details
{
    public class BranchGroupDto
    {
        public string Type { get; set; } = string.Empty;
        public List<Branch>? Branches { get; set; }
    }
}
