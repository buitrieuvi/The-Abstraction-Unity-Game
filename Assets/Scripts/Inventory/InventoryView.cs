
using DG.Tweening;
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
    [SerializeField] private Button closeButton;
    [SerializeField] private ViewBase avatar;

    private List<InventoryCategoryView> inventoryCategoryViews =
        new List<InventoryCategoryView>();

    public Transform InventoryCategoryViews;
    private InventoryCategoryView inventoryCategoryView;

    private readonly List<InventoryItemView> inventoryItems = new();
    private InventoryItemView selectedItem;

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

        if (InventoryCategoryViews != null)
        {
            inventoryCategoryViews =
                InventoryCategoryViews
                    .GetComponentsInChildren<InventoryCategoryView>(true)
                    .ToList();

            foreach (var cat in inventoryCategoryViews)
            {
                cat.OnEnter += (e) => cat.Dotween_Ani_HoverEnter();
                cat.OnExit += (e) => cat.Dotween_Ani_HoverExit();
            }
        }

        foreach (var item in inventoryItems)
        {
            item.OnEnter += (e) => item.Dotween_Ani_HoverEnter();
            item.OnExit += (e) => item.Dotween_Ani_HoverExit();

            item.Dotween_Ani_ScaleX(time, spaceTime);
        }

        base.Open(ctrl);

        if (ctrl is not InventoryController inventory)
            return;

        selectedItem?.Deselect();
        selectedItem = null;

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

        SelectFirstVisibleItem();


        SetBtnCat(inventory, 0, "");
        SetBtnCat(inventory, 1, "f");
        SetBtnCat(inventory, 2, "m");

        inventoryCategoryViews[0]?.PointerClick();

        foreach (var itemView in inventoryItems)
        {
            itemView.OnClick += e =>
            {
                inventory.UpdateItem(inventoryItems.IndexOf(itemView), 1);
                if (itemView.IsEmpty || itemView == selectedItem)
                    return;

                selectedItem?.Deselect();
                selectedItem = itemView;
                OnSelect();
            };
        }


        if (closeButton != null)
        {
            closeButton.onClick.AddListener(async () => await viewManager.ShowView<InventoryView>(inventory));
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnSelect()
    {
        if (selectedItem == null || selectedItem.IsEmpty)
            return;

        selectedItem.Select();

        if (avatar != null && selectedItem.AvatarImage != null)
        {
            avatar.GetComponent<Image>().sprite = selectedItem.AvatarImage.sprite;
            avatar.Dotween_Ani_ScaleX(time, spaceTime);

        }
    }

    public void SetBtnCat(
        InventoryController inventory, int index, string tag)
    {
        if (inventory == null ||
            index < 0 ||
            index >= inventoryCategoryViews.Count)
            return;

        var categoryView = inventoryCategoryViews[index];

        if (categoryView.OnClick != null)
            return;

        categoryView.OnClick += e =>
        {

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

            selectedItem?.Deselect();
            selectedItem = null;

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

            inventoryItems[0]?.PointerClick();

            SelectFirstVisibleItem();

            if (inventoryCategoryView != null) 
            {
                inventoryCategoryView.OnOutline(false);
            }
            inventoryCategoryView = categoryView;
            inventoryCategoryView.OnOutline(true);
        };


    }

    private void SelectFirstVisibleItem()
    {
        selectedItem = inventoryItems.FirstOrDefault(
            item => item != null && !item.IsEmpty);

        if (selectedItem != null)
        {
            OnSelect();
        }
        else if (avatar != null)
        {
            avatar.GetComponent<Image>().sprite = null;
        }
    }
}