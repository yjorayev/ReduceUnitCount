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
                .ContinueWith(task => ReduceUnitsForProductBottomUp(items, productName, task.Result));
        }

        private static List<OrderItem> ReduceUnitsForProductRecursive(IEnumerable<OrderItem> items, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures)
        {
            var uOM = unitOfMeasures.ToDictionary(unitOfMeasure => unitOfMeasure.Name);
            var total = items.Sum(item => item.Quantity * uOM[item.UnitOfMeasure].SinglesPerUnit);

            var memo = new Dictionary<int, IDictionary<string, OrderItem>?>();
            return ReduceUnitsInternalRecursive(total, productName, unitOfMeasures, memo)!
                .Values
                .ToList();
        }

        private static List<OrderItem> ReduceUnitsForProductBottomUp(IEnumerable<OrderItem> items, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures)
        {
            var uOM = unitOfMeasures.ToDictionary(unitOfMeasure => unitOfMeasure.Name);
            var total = items.Sum(item => item.Quantity * uOM[item.UnitOfMeasure].SinglesPerUnit);

            return ReduceUnitsInternalBottomUp(total, productName, unitOfMeasures)!
                .Values
                .ToList();
        }

        private static IDictionary<string, OrderItem>? ReduceUnitsInternalRecursive(int totalCount, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures, Dictionary<int, IDictionary<string, OrderItem>?> memo)
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
                    var temp = ReduceUnitsInternalRecursive(totalCount - unitOfMeasure.SinglesPerUnit, productName, unitOfMeasures, memo);
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

        private static Dictionary<string, OrderItem> ReduceUnitsInternalBottomUp(int totalCount, string productName, IEnumerable<UnitOfMeasure> unitOfMeasures)
        {
            var result = new Dictionary<string, OrderItem>[totalCount + 1];
            result[0] = new Dictionary<string, OrderItem>();

            for (int i = 1; i <= totalCount; i++)
            {
                foreach (var unitOfMeasure in unitOfMeasures)
                {
                    if (unitOfMeasure.SinglesPerUnit <= i) 
                    {
                        var currMin = result[i - unitOfMeasure.SinglesPerUnit];

                        if(currMin != null && TotalCount(currMin)+1 < TotalCount(result[i]))
                        {
                            result[i] = new Dictionary<string, OrderItem>(currMin);
                            if(currMin != null && currMin.TryGetValue(unitOfMeasure.Name, out var item))
                            {
                                result[i][unitOfMeasure.Name] = item with { Quantity = item.Quantity + 1 };
                            }
                            else
                            {
                                result[i][unitOfMeasure.Name] = new OrderItem(productName, unitOfMeasure.Name, 1);
                            }
                        }
                    }
                }
            }

            return result[totalCount];
        }

        private static int TotalCount(Dictionary<string, OrderItem> items)
        {
            if (items == null)
                return int.MaxValue;

            return items.Values.Sum(item => item.Quantity);
        }
    }
}
