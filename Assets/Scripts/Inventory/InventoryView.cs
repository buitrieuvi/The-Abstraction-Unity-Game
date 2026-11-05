
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using ModestTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryView : ViewBase
{
    [Inject] private DiContainer container;

    [SerializeField] float time;
    [SerializeField] float spaceTime;

    [SerializeField] private Transform itemContainer;
    [SerializeField] private ViewBase closeButton;
    [SerializeField] private ViewBase avatar;

    private List<InventoryItemView> inventoryItems = new();
    private InventoryItemView inventoryItemSelected = new();

    [SerializeField] private InventoryCategoryView[] inventoryCategoryView;
    [SerializeField] private InventoryCategoryView inventoryCategorySelectedView;

    public override void Awake()
    {
        base.Awake();
    }
    public override void Open(ControllerBase ctrl)
    {

        if (itemContainer != null)
        {
            itemContainer.GetComponentsInChildren<InventoryItemView>(
                true, inventoryItems);
        }

        foreach (var item in inventoryItems)
        {
            item.OnEnter += (e) => item.Dotween_Ani_HoverEnter(1.05f);
            item.OnExit += (e) => item.Dotween_Ani_HoverExit();
            item.OnClick += e =>
            {
                if (item.IsEmpty || item == inventoryItemSelected)
                    return;

                if (inventoryItemSelected != null)
                {
                    inventoryItemSelected.OnDeSelect();
                }
                inventoryItemSelected = item;
                inventoryItemSelected.OnSelect();   
                if (avatar != null && inventoryItemSelected.AvatarImage != null)
                {
                    avatar.GetComponent<Image>().sprite = inventoryItemSelected.AvatarImage.sprite;
                    avatar.Dotween_Ani_ScaleX(0.025f, spaceTime);
                }
            };

            item.Dotween_Ani_ScaleX(time, spaceTime);
        }

        base.Open(ctrl);

        if (ctrl is not InventoryController inventory)
            return;


        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (i >= inventory.InventoryModel.ItemCtrls.Count)
            {
                inventoryItems[i].Open(null);
                continue;
            }

            var itemCtrl = inventory.InventoryModel.ItemCtrls[i];

            inventoryItems[i].Open(itemCtrl);
        }

        inventoryItems[0]?.PointerClick();

        inventoryCategoryView?.ForEach(e =>
        {
            e.OnClick += _ =>
            {
                inventoryItems.ForEach(e => e.Open(null));

                InventoryCategoryView categoryView = e as InventoryCategoryView;

                inventoryCategorySelectedView?.OnOutline(false);
                inventoryCategorySelectedView = categoryView;
                inventoryCategorySelectedView.OnOutline(true);

                string tag = categoryView.CategoryName;

                if (tag == "") 
                {
                    inventoryItems.ForEach(e => e.Open(null));
                    return;
                }

                //if (tag == "item") 
                //{

                //    for (int i = 0; i < inventoryItems.Count; i++)
                //    {
                //        if (i >= inventory.InventoryModel.ItemCtrls.Count)
                //        {
                //            inventoryItems[i].Open(null);
                //            inventoryItems[i].IndexId = -1;
                //            continue;
                //        }

                //        var itemCtrl = inventory.InventoryModel.ItemCtrls[i];

                //        inventoryItems[i].Open(
                //            itemCtrl.IsShow ? inventory.GetByIndex(i) : null);

                //        inventoryItems[i].IndexId = i;

                //        inventoryItems[i].Dotween_Ani_ScaleX(time, spaceTime);
                //    }

                //    return;
                //};

                InventoryController inventorySort = new();

                int index = 0;

                inventory.InventoryModel.ItemCtrls.ForEach(item => 
                {
                    ItemSO itemso = dataManager.GetItem(item.ItemModel.Id);
                    List<CategorySO> itemCategorys = itemso.ItemCategorys;
                    itemCategorys.ForEach(category =>
                    {
                        if (category.CategoryName == tag)
                        {
                            inventorySort.InventoryModel.ItemCtrls.Add(item);
                            inventoryItems[index].Open(item);
                            inventoryItems[index].IndexId = inventory.InventoryModel.ItemCtrls.IndexOf(item);
                            index++;
                            //ChangedItem(item.ItemModel.Id, item.ItemModel.Quantity);                            
                        }
                    });



                });


                //for (int i = 0; i < inventoryItems.Count; i++)
                //{
                //    if (i >= inventorySort.InventoryModel.ItemCtrls.Count)
                //    {
                //        inventoryItems[i].Open(null);
                //        inventoryItems[i].IndexId = -1;
                //        continue;
                //    }

                //    var itemCtrl = inventorySort.InventoryModel.ItemCtrls[i];

                //    inventoryItems[i].Open(
                //        itemCtrl.IsShow ? inventorySort.GetByIndex(i) : null);

                //    inventoryItems[i].IndexId = i;
                //    inventoryItems[i].Dotween_Ani_ScaleX(time, spaceTime);
                //}
            };
        });

        inventoryCategoryView[0]?.PointerClick();

        if (closeButton != null)
        {
            closeButton.OnClick += async (e) => await viewManager.ShowView<InventoryView>(inventory);
        }

        inputManager.InputActions.UI.Inventory.started += async (e) =>
        {
            await viewManager.ShowView<InventoryView>(inventory);
        };
    }

    public override void Close()
    {
        base.Close();

        inputManager.InputActions.UI.Inventory.Dispose();
    }
}