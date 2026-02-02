public class ProductListItem
{
    public int ProductId { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RecommendedPrice { get; set; }
    public int StatusId { get; set; }
}