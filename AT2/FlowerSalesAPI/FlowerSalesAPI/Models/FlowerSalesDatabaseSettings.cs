namespace FlowerSalesAPI.Models
{
    public class FlowerSalesDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ProductCollectionName { get; set; } = null!;
    }
}
