
public class InventoryItemController : ControllerBase 
{
    public ItemModel ItemModel = new();

    public bool IsShow = true;

    public InventoryItemController(string id, int quantity) 
    {
        ItemModel.Id = id;
        ItemModel.Quantity = quantity;
    }
}
