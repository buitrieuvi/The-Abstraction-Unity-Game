using DG.Tweening;
using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class InventoryItemView : ViewBase
{

    [SerializeField] private GameObject emptyContainer;
    [SerializeField] private GameObject itemDataContainer;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Image rankImage;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Outline outline;

    public Image AvatarImage => avatarImage;
    private Tween scaleTween;
    private IDisposable itemSub;
    public bool IsEmpty
    {
        get
        {
            return emptyContainer.activeSelf;
        }
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void PointerEnter()
    {
        base.PointerEnter();
    }

    public override void PointerExit()
    {
        base.PointerExit();
   
    }

    public override void PointerClick()
    {
        base.PointerClick();
    }

public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);

        // Hủy subscription cũ trước khi đăng ký mới.
        itemSub?.Dispose();
        itemSub = null;

        if (ctrl is not InventoryItemController item)
        {
            ShowEmpty();
            return;
        }

        RefreshItem(item);

        itemSub = Observable
            .EveryValueChanged(item, x => x.ItemModel.Quantity)
            .Subscribe(_ =>
            {
                // Không cập nhật nếu View đã bị hủy.
                if (this == null || gameObject == null)
                    return;

                RefreshItem(item);
            });
    }

    public override void Close()
    {
        itemSub?.Dispose();
        itemSub = null;

        scaleTween?.Kill();
        scaleTween = null;

        base.Close();
    }

    private void OnDestroy()
    {
        itemSub?.Dispose();
        itemSub = null;

        scaleTween?.Kill();
        scaleTween = null;
    }

private void ShowEmpty()
    {
        if (emptyContainer != null)
            emptyContainer.SetActive(true);

        if (itemDataContainer != null)
            itemDataContainer.SetActive(false);
    }

    private void RefreshItem(InventoryItemController item)
    {
        if (this == null || gameObject == null)
            return;

        if (item == null || item.ItemModel == null)
        {
            ShowEmpty();
            return;
        }

        var model = item.ItemModel;

        if (model.Quantity <= 0 || string.IsNullOrEmpty(model.Id))
        {
            ShowEmpty();
            return;
        }

        ItemSO so = dataManager.GetItem(model.Id);

        if (so == null)
        {
            ShowEmpty();
            return;
        }

        if (emptyContainer == null || itemDataContainer == null ||
            avatarImage == null || rankImage == null ||
            quantityText == null)
            return;

        emptyContainer.SetActive(false);
        itemDataContainer.SetActive(true);

        avatarImage.sprite = so.Avatar;
        rankImage.color = so.ItemRank.Color;
        quantityText.text = model.Quantity.ToString();
    }

    public void Select()
    {
        outline.enabled = true;
    }

    public void Deselect()
    {
        outline.enabled = false;
    }

    public void Animation(float delay) 
    {
        tfView.DOScale(1, 0.35f).SetDelay(delay * transform.GetSiblingIndex()).From(0).SetEase(Ease.InOutFlash);
    }
}
