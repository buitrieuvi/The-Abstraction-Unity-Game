
using UnityEngine;

public class Machine1UpgradeView : ViewUpgrade
{

    public InventoryView InventoryView;

    public ViewButtonBase Setbtn;

    //public UpgradeCategoryItemView InventoryCategoryViewSeletected;
    //public InventoryItemView InventoryItemViewSeletected;

    //public UpgradeCategoryItemView[] UpgradeCategoryItemViews;
    //public UpgradeCategoryItemView UpgradeCategoryItemView;

    public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);

        InventoryView = GetComponent<InventoryView>();
        InventoryView.Open(dataManager.PlayerInventory);

        if (ctrl is not MachineSpawnCubeController machineSpawnCubeController)
            return;

        Setbtn.OnClick += (e) =>
        {
            Debug.Log("lap trinh tiep cho nay");

            UpgradeCategoryItemView upgradeCategoryItemView = InventoryView.InventoryCategorySelectedView as UpgradeCategoryItemView;

            upgradeCategoryItemView.AvatarImage.gameObject.SetActive(true);
            upgradeCategoryItemView.AvatarImage.sprite = InventoryView.AvatarImage.sprite;
        };


        //foreach (var item in UpgradeCategoryItemViews)
        //{
        //    item.OnClick += (e) =>
        //    {
        //        if (UpgradeCategoryItemView != null)
        //        {
        //            UpgradeCategoryItemView.OnOutline(false);
        //        }

        //        UpgradeCategoryItemView = item;

        //        UpgradeCategoryItemView.OnOutline(true);
        //    };
        //}

        InventoryView.SeletedCat0();
        InventoryView.SeletedItem0();
    }

}
