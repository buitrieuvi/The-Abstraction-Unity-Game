
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

            inventoryItems[i].Open(
                itemCtrl.IsShow ? inventory.GetByIndex(i) : null);
        }

        //SelectFirstVisibleItem();


        if (closeButton != null)
        {
            closeButton.OnClick += async (e) => await viewManager.ShowView<InventoryView>(inventory);
        }


        inputManager.InputActions.UI.Inventory.started += async (e) => 
        {
            await viewManager.ShowView<InventoryView>(inventory);
        };

        inventoryItems[0]?.PointerClick();

        inventoryCategoryView?.ForEach(e =>
        {
            e.OnClick += e =>
            {
                Debug.Log($"InventoryCategoryView PointerClick: {e.gameObject.name}");

                InventoryCategoryView categoryView = e as InventoryCategoryView;

                inventoryCategorySelectedView?.OnOutline(false);
                inventoryCategorySelectedView = categoryView;
                inventoryCategorySelectedView.OnOutline(true);

                string tag = categoryView.CategoryName;

                var category = string.IsNullOrEmpty(tag)
                    ? null
                    : dataManager.Categories.FirstOrDefault(
                        x => x.CategoryName == tag);

                if (!string.IsNullOrEmpty(tag) && category == null)
                {
                    Debug.LogWarning(
                        $"Không tìm thấy danh mục: {tag}");
                    return;
                }

                int count = Mathf.Min(
                    inventoryItems.Count,
                    inventory.InventoryModel.ItemCtrls.Count);

                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    var itemView = inventoryItems[i];

                    if (i >= count)
                    {
                        itemView.Open(null);
                        continue;
                    }

                    var itemCtrl = inventory.InventoryModel.ItemCtrls[i];
                    var itemSO = dataManager.GetItem(itemCtrl.ItemModel.Id);

                    bool show = string.IsNullOrEmpty(tag) ||
                        (itemSO != null &&
                         itemSO.ItemCategorys != null &&
                         itemSO.ItemCategorys.Contains(category));

                    itemCtrl.IsShow = show;

                    if (show)
                    {
                        itemView.Open(inventory.GetByIndex(i));
                        itemView.Dotween_Ani_ScaleX(time, spaceTime);
                    }

                    else
                        itemView.Close();
                }

                inventoryItems.First(e => e.gameObject.activeSelf).PointerClick();
            };
        });

        inventoryCategoryView[0]?.PointerClick();
    }

    public override void Close()
    {
        base.Close();

        inputManager.InputActions.UI.Inventory.Dispose();
    }
}