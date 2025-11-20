using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Inventory (Kho vật tư / thiết bị)
    /// </summary>
    public class Inventory
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; } // Material, Equipment
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Supplier { get; set; }

        public Inventory() { }

        public Inventory(int itemId, string itemName, string type,
            int quantity, string unit, string supplier)
        {
            ItemId = itemId;
            ItemName = itemName;
            Type = type;
            Quantity = quantity;
            Unit = unit;
            Supplier = supplier;
        }

        /// <summary>
        /// Kiểm tra có phải vật tư không
        /// </summary>
        public bool IsMaterial()
        {
            return Type?.ToLower() == "material";
        }

        /// <summary>
        /// Kiểm tra có phải thiết bị không
        /// </summary>
        public bool IsEquipment()
        {
            return Type?.ToLower() == "equipment";
        }

        /// <summary>
        /// Kiểm tra sắp hết hàng (dưới 20)
        /// </summary>
        public bool IsLowStock()
        {
            return Quantity < 20;
        }

        /// <summary>
        /// Lấy loại hiển thị
        /// </summary>
        public string GetTypeDisplay()
        {
            return IsMaterial() ? "Vật tư" : "Thiết bị";
        }

        public override string ToString()
        {
            return $"{ItemName} ({Quantity} {Unit})";
        }
    }
}