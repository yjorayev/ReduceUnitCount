namespace ProductManagement
{
    public class Order
    {
        public static Task<List<OrderItem>> ReduceUnitsAsync(IEnumerable<OrderItem> items, IProductStore store)
        {
            var tasks = items
                .GroupBy(item => item.ProductName)
                .Select(group => ReduceUnitsForProductAsync(group, group.Key, store));

            return Task
                .WhenAll(tasks)
                .ContinueWith(task => task.Result.SelectMany(x => x).ToList());
        }

        private static Task<List<OrderItem>> ReduceUnitsForProductAsync(IEnumerable<OrderItem> items, string productName, IProductStore store)
        {
            return store
                .GetUnitsForProductAsync(productName)
                .ContinueWith(task => ReduceUnitsForProduct(items, productName, task.Result));
        }

        private static List<OrderItem> ReduceUnitsForProduct(IEnumerable<OrderItem> items, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures)
        {
            var uOM = unitOfMeasures.ToDictionary(unitOfMeasure => unitOfMeasure.Name);
            var total = items.Sum(item => item.Quantity * uOM[item.UnitOfMeasure].SinglesPerUnit);

            var memo = new Dictionary<int, IDictionary<string, OrderItem>?>();
            return ReduceUnitsInternal(total, productName, unitOfMeasures, memo)!
                .Values
                .ToList();
        }

        private static IDictionary<string, OrderItem>? ReduceUnitsInternal(int totalCount, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures, Dictionary<int, IDictionary<string, OrderItem>?> memo)
        {
            if (totalCount == 0)
                return new Dictionary<string, OrderItem>();

            if (memo.TryGetValue(totalCount, out IDictionary<string, OrderItem>? currMin))
            {
                return currMin;
            }

            int minUnitCount = 0;
            foreach (var unitOfMeasure in unitOfMeasures)
            {
                if (unitOfMeasure.SinglesPerUnit <= totalCount)
                {
                    var temp = ReduceUnitsInternal(totalCount - unitOfMeasure.SinglesPerUnit, productName, unitOfMeasures, memo);
                    if (temp == null)
                        continue;

                    int unitCount = temp.Sum(item => item.Value.Quantity);

                    if (currMin == null || unitCount + 1 < minUnitCount)
                    {
                        if (temp.TryGetValue(unitOfMeasure.Name, out var orderItem))
                        {
                            temp[unitOfMeasure.Name] = orderItem with { Quantity = orderItem.Quantity + 1 };
                        }
                        else
                        {
                            temp[unitOfMeasure.Name] = new OrderItem(productName, unitOfMeasure.Name, 1);
                        }

                        currMin = temp;
                        minUnitCount = unitCount + 1;
                    }
                }
            }

            memo[totalCount] = currMin == null ? null : new Dictionary<string, OrderItem>(currMin);
            return currMin;
        }
    }
}
