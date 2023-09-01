namespace Basket.Core.Entities;

public class ShoppingCartItem
{
    public int Quantitiy { get; set; }
    public decimal Price { get; set; }
    public string ProductId { get; set; }
    public string ImageFile { get; set; }
    public string ProductName { get; set; }
}