
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryView : ViewBase
{
    [SerializeField] private float animationDuration = 0.5f;

    [Inject] private DiContainer container;


    [SerializeField] private Transform itemContainer;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image avatar;

    private List<InventoryCategoryView> inventoryCategoryViews =
        new List<InventoryCategoryView>();

    public Transform InventoryCategoryViews;
    private InventoryCategoryView inventoryCategoryView;

    private readonly List<InventoryItemView> inventoryItems = new();
    private InventoryItemView selectedItem;

    public override void Awake()
    {
        base.Awake();

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
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => viewManager.OnlyClose());
        }

        foreach (var itemView in inventoryItems)
        {
            itemView.OnEnter += e =>
            {
                if (!itemView.IsEmpty)
                    itemView.Dotween_1();
            };

            itemView.OnExit += e =>
            {
                if (!itemView.IsEmpty)
                    itemView.Dotween_2();
            };

            itemView.OnClick += e =>
            {
                if (itemView.IsEmpty || itemView == selectedItem)
                    return;

                selectedItem?.Deselect();
                selectedItem = itemView;
                OnSelect();
            };
        }
    }

    public override void Open(ControllerBase ctrl)
    {
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
        //AnimateItems();

        SetBtnCat(inventory, 0, "");
        SetBtnCat(inventory, 1, "f");
        SetBtnCat(inventory, 2, "m");

        inventoryCategoryViews[0]?.PointerClick();
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
            avatar.sprite = selectedItem.AvatarImage.sprite;

            avatar.transform
                .DOScale(1f, 0.2f)
                .SetEase(Ease.OutBack)
                .From(0f)
                .SetUpdate(true);
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

            // Không lọc nếu tên danh mục không hợp lệ.
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
                    itemView.Open(inventory.GetByIndex(i));
                else
                    itemView.Close();
            }

            inventoryItems[0]?.PointerClick();

            SelectFirstVisibleItem();
            AnimateItems();

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
            avatar.sprite = null;
        }
    }

    private void AnimateItems()
    {
        foreach (var item in inventoryItems)
        {
            item.Animation(animationDuration);
        }
    }
}