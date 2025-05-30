namespace WarehouseManagement.DTOs.Request;

public class ChangePasswordEmployeeDto
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
