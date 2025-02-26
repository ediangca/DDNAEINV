namespace DDNAEINV.Model.Details
{
    public class ChangePassDto
    {
        public string? OldPassword{ get; set; }
        public required string NewPassword { get; set; }
    }
}
