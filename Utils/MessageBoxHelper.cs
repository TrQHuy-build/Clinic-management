using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class helper cho việc hiển thị MessageBox
    /// </summary>
    public static class MessageBoxHelper
    {
        /// <summary>
        /// Hiển thị thông báo thành công
        /// </summary>
        public static void ShowSuccess(string message, string title = "Thành công")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        public static void ShowError(string message, string title = "Lỗi")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị cảnh báo
        /// </summary>
        public static void ShowWarning(string message, string title = "Cảnh báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Hiển thị thông tin
        /// </summary>
        public static void ShowInfo(string message, string title = "Thông tin")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị xác nhận Yes/No
        /// </summary>
        public static bool ShowConfirm(string message, string title = "Xác nhận")
        {
            DialogResult result = MessageBox.Show(message, title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Hiển thị xác nhận Yes/No/Cancel
        /// </summary>
        public static DialogResult ShowConfirmWithCancel(string message, string title = "Xác nhận")
        {
            return MessageBox.Show(message, title,
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Hiển thị thông báo xóa
        /// </summary>
        public static bool ShowDeleteConfirm(string itemName)
        {
            return ShowConfirm($"Bạn có chắc chắn muốn xóa {itemName}?\nHành động này không thể hoàn tác!",
                "Xác nhận xóa");
        }

        /// <summary>
        /// Hiển thị thông báo lưu thành công
        /// </summary>
        public static void ShowSaveSuccess()
        {
            ShowSuccess("Lưu dữ liệu thành công!");
        }

        /// <summary>
        /// Hiển thị thông báo thêm thành công
        /// </summary>
        public static void ShowAddSuccess(string itemName = "")
        {
            string message = string.IsNullOrEmpty(itemName)
                ? "Thêm mới thành công!"
                : $"Thêm {itemName} thành công!";
            ShowSuccess(message);
        }

        /// <summary>
        /// Hiển thị thông báo cập nhật thành công
        /// </summary>
        public static void ShowUpdateSuccess(string itemName = "")
        {
            string message = string.IsNullOrEmpty(itemName)
                ? "Cập nhật thành công!"
                : $"Cập nhật {itemName} thành công!";
            ShowSuccess(message);
        }

        /// <summary>
        /// Hiển thị thông báo xóa thành công
        /// </summary>
        public static void ShowDeleteSuccess(string itemName = "")
        {
            string message = string.IsNullOrEmpty(itemName)
                ? "Xóa thành công!"
                : $"Xóa {itemName} thành công!";
            ShowSuccess(message);
        }

        /// <summary>
        /// Hiển thị validation error
        /// </summary>
        public static void ShowValidationError(string fieldName)
        {
            ShowWarning($"Vui lòng nhập {fieldName}!", "Thiếu thông tin");
        }

        /// <summary>
        /// Hiển thị validation error tùy chỉnh
        /// </summary>
        public static void ShowValidationError(string message, string title = "Lỗi nhập liệu")
        {
            ShowWarning(message, title);
        }

        /// <summary>
        /// Hiển thị lỗi database
        /// </summary>
        public static void ShowDatabaseError(string operation = "")
        {
            string message = string.IsNullOrEmpty(operation)
                ? "Có lỗi xảy ra khi thực hiện thao tác với database!"
                : $"Có lỗi xảy ra khi {operation}!";
            ShowError(message, "Lỗi Database");
        }

        /// <summary>
        /// Hiển thị thông báo không tìm thấy dữ liệu
        /// </summary>
        public static void ShowNotFound(string itemName)
        {
            ShowWarning($"Không tìm thấy {itemName}!", "Không tìm thấy");
        }

        /// <summary>
        /// Hiển thị thông báo trùng dữ liệu
        /// </summary>
        public static void ShowDuplicateError(string fieldName)
        {
            ShowError($"{fieldName} đã tồn tại trong hệ thống!", "Dữ liệu trùng lặp");
        }

        /// <summary>
        /// Hiển thị thông báo không có quyền
        /// </summary>
        public static void ShowAccessDenied()
        {
            ShowError("Bạn không có quyền thực hiện chức năng này!", "Truy cập bị từ chối");
        }
    }
}