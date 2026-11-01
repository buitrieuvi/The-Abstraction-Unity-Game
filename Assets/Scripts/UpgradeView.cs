using UnityEngine;

public class ViewUpgrade : ViewBase
{
    private InventoryView inventoryView;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnChanged()
    {
        base.OnChanged();
    }

    public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);
        inventoryView = GetComponent<InventoryView>();
        inventoryView.Open(dataManager.PlayerInventory);

    }

    public override void PointerClick()
    {
        base.PointerClick();
    }

    public override void PointerEnter()
    {
        base.PointerEnter();
    }

    public override void PointerExit()
    {
        base.PointerExit();
    }
}
