using CocoaAtelierPlus.Models;
using Microsoft.Data.Sqlite;

namespace CocoaAtelierPlus.Services
{
    public static class MaterialService
    {
        public static List<Material> GetAll()
        {
            var result = new List<Material>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT m.material_id, m.material_type_id, mt.name, m.name, m.unit_price,
                       m.stock_qty, m.min_qty, m.pack_qty, m.unit
                FROM material m
                JOIN material_type mt ON mt.material_type_id = m.material_type_id
                ORDER BY m.name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var material = new Material
                {
                    MaterialId = reader.GetInt32(0),
                    MaterialTypeId = reader.GetInt32(1),
                    MaterialTypeName = reader.GetString(2),
                    Name = reader.GetString(3),
                    UnitPrice = reader.GetDecimal(4),
                    StockQty = reader.GetDecimal(5),
                    MinQty = reader.GetDecimal(6),
                    PackQty = reader.GetDecimal(7),
                    Unit = reader.GetString(8)
                };
                material.MinBatchQty = CalculationService.MinBatchQty(material);
                material.MinBatchCost = CalculationService.MinBatchCost(material);
                material.RequiredQty = CalculationService.RequiredQty(material.MaterialId);
                result.Add(material);
            }

            return result;
        }

        public static Material? GetById(int materialId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT m.material_id, m.material_type_id, mt.name, m.name, m.unit_price,
                       m.stock_qty, m.min_qty, m.pack_qty, m.unit
                FROM material m
                JOIN material_type mt ON mt.material_type_id = m.material_type_id
                WHERE m.material_id = @id";
            command.Parameters.AddWithValue("@id", materialId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var material = new Material
            {
                MaterialId = reader.GetInt32(0),
                MaterialTypeId = reader.GetInt32(1),
                MaterialTypeName = reader.GetString(2),
                Name = reader.GetString(3),
                UnitPrice = reader.GetDecimal(4),
                StockQty = reader.GetDecimal(5),
                MinQty = reader.GetDecimal(6),
                PackQty = reader.GetDecimal(7),
                Unit = reader.GetString(8)
            };
            material.MinBatchQty = CalculationService.MinBatchQty(material);
            material.MinBatchCost = CalculationService.MinBatchCost(material);
            material.RequiredQty = CalculationService.RequiredQty(material.MaterialId);
            return material;
        }

        public static List<MaterialTypeItem> GetTypes()
        {
            var result = new List<MaterialTypeItem>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT material_type_id, name FROM material_type ORDER BY name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new MaterialTypeItem
                {
                    MaterialTypeId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return result;
        }

        public static void Add(MaterialForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO material (material_type_id, name, unit_price, stock_qty, min_qty, pack_qty, unit)
                VALUES (@type, @name, @price, @stock, @min, @pack, @unit)";
            command.Parameters.AddWithValue("@type", form.MaterialTypeId ?? 0);
            command.Parameters.AddWithValue("@name", form.Name);
            command.Parameters.AddWithValue("@price", form.UnitPrice ?? 0);
            command.Parameters.AddWithValue("@stock", form.StockQty ?? 0);
            command.Parameters.AddWithValue("@min", form.MinQty ?? 0);
            command.Parameters.AddWithValue("@pack", form.PackQty ?? 0);
            command.Parameters.AddWithValue("@unit", form.Unit);
            command.ExecuteNonQuery();
        }

        public static void Update(MaterialForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE material
                SET material_type_id = @type,
                    name = @name,
                    unit_price = @price,
                    stock_qty = @stock,
                    min_qty = @min,
                    pack_qty = @pack,
                    unit = @unit
                WHERE material_id = @id";
            command.Parameters.AddWithValue("@id", form.MaterialId ?? 0);
            command.Parameters.AddWithValue("@type", form.MaterialTypeId ?? 0);
            command.Parameters.AddWithValue("@name", form.Name);
            command.Parameters.AddWithValue("@price", form.UnitPrice ?? 0);
            command.Parameters.AddWithValue("@stock", form.StockQty ?? 0);
            command.Parameters.AddWithValue("@min", form.MinQty ?? 0);
            command.Parameters.AddWithValue("@pack", form.PackQty ?? 0);
            command.Parameters.AddWithValue("@unit", form.Unit);
            command.ExecuteNonQuery();
        }

        public static List<ProductItem> GetProducts(int materialId)
        {
            var result = new List<ProductItem>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT p.name, r.qty_per_unit
                FROM recipe r
                JOIN product p ON p.product_id = r.product_id
                WHERE r.material_id = @id
                ORDER BY p.name";
            command.Parameters.AddWithValue("@id", materialId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ProductItem
                {
                    Name = reader.GetString(0),
                    QtyPerUnit = reader.GetDecimal(1)
                });
            }

            return result;
        }

        public static List<SupplierItem> GetSuppliers(int materialId)
        {
            var result = new List<SupplierItem>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT s.supplier_id, s.name, s.rating, s.start_date, ms.purchase_price, ms.delivery_days
                FROM material_supplier ms
                JOIN supplier s ON s.supplier_id = ms.supplier_id
                WHERE ms.material_id = @id
                ORDER BY s.name";
            command.Parameters.AddWithValue("@id", materialId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new SupplierItem
                {
                    SupplierId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Rating = reader.GetInt32(2),
                    StartDate = reader.GetString(3),
                    PurchasePrice = reader.GetDecimal(4),
                    DeliveryDays = reader.GetInt32(5)
                });
            }

            return result;
        }

        public static bool NameExists(string name, int? materialId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*)
                FROM material
                WHERE name = @name AND material_id <> @id";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@id", materialId ?? 0);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}
