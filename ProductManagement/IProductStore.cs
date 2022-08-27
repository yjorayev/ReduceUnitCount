namespace ProductManagement
{
    public interface IProductStore
    {
        Task<List<UnitOfMeasure>> GetUnitsForProductAsync(string productName);
    }
}