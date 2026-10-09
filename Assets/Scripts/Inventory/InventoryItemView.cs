using DG.Tweening;
using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryItemView : ViewBase
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
        InventoryItemController item = ctrl as InventoryItemController;

        if (ctrl == null)
        {
            emptyContainer.SetActive(true);
            itemDataContainer.SetActive(false);

            //itemSub?.Dispose();
            //itemSub = null;
        }
        else 
        {
            emptyContainer.SetActive(false);
            itemDataContainer.SetActive(true);

            ItemSO so = dataManager.GetItem(item.ItemModel.Id);

            avatarImage.sprite = so.Avatar;
            rankImage.color = so.ItemRank.Color;
            quantityText.text = item.ItemModel.Quantity.ToString();


            //itemSub = Observable.EveryValueChanged(item, x => x.ItemModel)
            //    .Subscribe((e) =>
            //    {
            //        if (item.ItemModel.Quantity == 0 || item.ItemModel.Id == "")
            //        {
            //            emptyContainer.SetActive(true);
            //            itemDataContainer.SetActive(false);
            //        }
            //        else
            //        {
            //            emptyContainer.SetActive(false);
            //            itemDataContainer.SetActive(true);

            //            avatarImage.sprite = so.Avatar;
            //            rankImage.color = so.ItemRank.Color;
            //            quantityText.text = item.ItemModel.Quantity.ToString();
            //        }
            //    })
            //    .AddTo(this);
        };



    }

    public override void Close()
    {
        base.Close();
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
