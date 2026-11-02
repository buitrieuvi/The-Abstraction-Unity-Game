using UnityEngine;

public class ViewUpgrade : ViewBase
{
    private InventoryView inventoryView;

    public UpgradeItemView[] UpgradeItemViews;
    public UpgradeItemView UpgradeItemSeleted;

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

        foreach (var item in UpgradeItemViews)
        {
            item.OnClick += (e) => 
            {
                if (UpgradeItemSeleted != null)
                {
                    UpgradeItemSeleted.OnOutline(false);
                }

                UpgradeItemSeleted = item;
                UpgradeItemSeleted.OnOutline(true);

                //inventoryView.
            };


        }
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
