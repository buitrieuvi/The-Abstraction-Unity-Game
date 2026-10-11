
using System.Linq;
using UnityEngine;

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

        if (ex.ItemModel.Quantity + quantity < 0)
            return;

        ex.ItemModel.Quantity += quantity;
    }

    public InventoryItemController GetByIndex(int index)
    {
        if (!IsValidIndex(index))
            return null;

        return InventoryModel.ItemCtrls[index];
    }

    public InventoryItemController GetById(string id)
    {
        return InventoryModel.ItemCtrls
            .FirstOrDefault(x => x.ItemModel.Id == id);
    }

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
            if (it.ItemModel.Quantity + quantity <= 0)
                return;

            it.ItemModel.Quantity += quantity;
        }
    }

    public void UpdateItem(int index, int quantity)
    {
        if (!IsValidIndex(index))
        {
            Debug.LogError(
                $"InventoryController.UpdateItem: " +
                $"index={index}, Count={InventoryModel.ItemCtrls.Count}"
            );
            return;
        }

        var item = InventoryModel.ItemCtrls[index];

        if (item == null || item.ItemModel == null)
            return;

        int newQuantity = item.ItemModel.Quantity + quantity;

        if (newQuantity < 0)
            return;

        item.ItemModel.Quantity = newQuantity;
    }

    private bool IsValidIndex(int index)
    {
        return InventoryModel.ItemCtrls != null
            && index >= 0
            && index < InventoryModel.ItemCtrls.Count;
    }

    public int Count()
    {
        return InventoryModel.ItemCtrls.Count;
    }

    public InventoryItemController GetInventoryItemController(int index)
    {
        return GetByIndex(index);
    }
}