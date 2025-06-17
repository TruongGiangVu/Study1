using System.ComponentModel.DataAnnotations;

namespace GenApi.Constants;

/// <summary> Mã lỗi </summary>
public enum ErrorCode
{
    [Display(Name = "Thành công")] Success = 00,
    [Display(Name = "Không tìm thấy")] NotFound = 01,
    [Display(Name = "Đã tồn tại")] Exist = 02,
    [Display(Name = "Bắt buộc")] Required = 03,
    [Display(Name = "Sai đặc tả dữ liệu")] Validate = 04,
    [Display(Name = "Sai tài khoản hoặc mật khẩu")] InvalidUserPass = 42,
    [Display(Name = "Lỗi database")] Database = 98,
    [Display(Name = "Lỗi không xác định")] UnKnow = 99,
}
