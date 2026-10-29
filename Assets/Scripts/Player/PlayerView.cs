using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerView : ViewBase
{
    [SerializeField] TextMeshProUGUI txtMoney;
    [SerializeField] TextMeshProUGUI txtMoneyEf;

    IDisposable itemSub;

    public void Start()
    {
        itemSub = Observable
        .EveryValueChanged(dataManager, x => x.Money)
        .Subscribe(async _ =>
        {
            Ani_TextMoney_Fade(dataManager.MoneyCube.ToString());

            txtMoney.text = dataManager.Money.ToString();
        });

        txtMoneyEf.gameObject.SetActive(false);
    }

    public async void Ani_TextMoney_Fade(string m) 
    {
        txtMoneyEf.gameObject.SetActive(true);
        txtMoneyEf.text = m;
    }
}
