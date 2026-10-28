using CocoaAtelierPlus.Models;

namespace CocoaAtelierPlus.Services
{
    public static class CalculationService
    {
        public static decimal MinBatchQty(Material material)
        {
            if (material.StockQty >= material.MinQty || material.PackQty <= 0)
            {
                return 0;
            }

            var deficit = material.MinQty - material.StockQty;
            // Материал продается упаковками, поэтому остаток добираем до целого числа упаковок.
            var packs = Math.Ceiling(deficit / material.PackQty);
            return packs * material.PackQty;
        }

        public static decimal MinBatchCost(Material material)
        {
            var qty = MinBatchQty(material);
            var cost = qty * material.UnitPrice;
            if (cost < 0)
            {
                cost = 0;
            }

            return Math.Round(cost, 2);
        }

        public static decimal RequiredQty(int materialId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(SUM(qty_per_unit), 0) FROM recipe WHERE material_id = @id";
            command.Parameters.AddWithValue("@id", materialId);

            return Convert.ToDecimal(command.ExecuteScalar());
        }

        public static int ProductYield(int productTypeId, int materialTypeId, int rawQty, double param1, double param2)
        {
            if (productTypeId <= 0 || materialTypeId <= 0 || rawQty <= 0 || param1 <= 0 || param2 <= 0)
            {
                return -1;
            }

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT coefficient FROM product_type WHERE product_type_id = @id";
            command.Parameters.AddWithValue("@id", productTypeId);

            var coefficientValue = command.ExecuteScalar();
            if (coefficientValue == null)
            {
                return -1;
            }

            var coefficient = Convert.ToDouble(coefficientValue);

            using var lossCommand = connection.CreateCommand();
            lossCommand.CommandText = "SELECT loss_percent FROM material_type WHERE material_type_id = @id";
            lossCommand.Parameters.AddWithValue("@id", materialTypeId);

            var lossValue = lossCommand.ExecuteScalar();
            if (lossValue == null)
            {
                return -1;
            }

            var lossPercent = Convert.ToDouble(lossValue);

            var perUnit = param1 * param2 * coefficient;
            if (perUnit <= 0)
            {
                return -1;
            }

            var lossFactor = 1 - lossPercent / 100;
            if (lossFactor <= 0)
            {
                return -1;
            }

            // Потери уменьшают полезное количество сырья, из которого считается выход продукции.
            var available = rawQty * lossFactor;
            return (int)Math.Floor(available / perUnit);
        }
    }
}
