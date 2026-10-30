
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using TMPro;
using UnityEngine;

public class PlayerView : ViewBase
{
    [SerializeField] private TextMeshProUGUI txtMoney;
    [SerializeField] private TextMeshProUGUI txtMoneyEf;

    [Header("Money Effect")]
    [SerializeField] private float moveDistance = 60f;
    [SerializeField] private float duration = 1.8f;

    private IDisposable itemSub;
    private Tween moneyTween;
    private RectTransform moneyEffectRect;
    private Vector2 originalPosition;

    private void Start()
    {
        moneyEffectRect = txtMoneyEf.rectTransform;
        originalPosition = moneyEffectRect.anchoredPosition;

        txtMoneyEf.gameObject.SetActive(false);

        itemSub = Observable
            .EveryValueChanged(dataManager, x => x.Money)
            .Subscribe(_ =>
            {
                Ani_TextMoney_Fade(dataManager.MoneyCube.ToString());

                txtMoney.text = dataManager.Money.ToString();
            });
    }

    public void Ani_TextMoney_Fade(string m)
    {
        moneyTween?.Kill();

        txtMoneyEf.gameObject.SetActive(true);

        txtMoneyEf.text = "+" + m;

        moneyEffectRect.anchoredPosition = originalPosition;

        Color color = txtMoneyEf.color;
        color.a = 1f;
        txtMoneyEf.color = color;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            moneyEffectRect.DOAnchorPosY(
                originalPosition.y + moveDistance,
                duration
            ).SetEase(Ease.OutCubic)
        );

        sequence.Join(
            txtMoneyEf.DOFade(0f, duration)
                .SetEase(Ease.InQuad)
        );

        sequence.OnComplete(() =>
        {
            txtMoneyEf.gameObject.SetActive(false);
            moneyEffectRect.anchoredPosition = originalPosition;
        });

        moneyTween = sequence;
    }

    private void OnDestroy()
    {
        itemSub?.Dispose();
        moneyTween?.Kill();
    }
}
