using System.Linq;

public class InventoryController : ControllerBase
{
    public InventoryModel InventoryModel = new();

    public void ChangedItem(string id, int quantity)
    {
        var ex = GetById(id);
        if (ex == null)
        {
            if (quantity <= 0) return;
            InventoryModel.ItemCtrls.Add(new(id, quantity));
            return;
        }

        if (ex.ItemModel.Quantity + quantity < 0) { return; }


        ex.ItemModel.Quantity += quantity;
    }
    public InventoryItemController GetByIndex(int index) { return InventoryModel.ItemCtrls[index]; }
    public InventoryItemController GetById(string id) { return InventoryModel.ItemCtrls.FirstOrDefault(x => x.ItemModel.Id == id); }
    public void UpdateItem(string id, int quantity)
    {
        var it = GetById(id);
        if (it == null)
        {
            if (quantity <= 0) return;
            InventoryModel.ItemCtrls.Add(new(id, quantity));
        }
        else
        {
            if (it.ItemModel.Quantity + quantity <= 0) return;
            it.ItemModel.Quantity += quantity;
        }
    }
    public int Count() => InventoryModel.ItemCtrls.Count;
    public InventoryItemController GetInventoryItemController(int index) { return InventoryModel.ItemCtrls[index]; }

}
