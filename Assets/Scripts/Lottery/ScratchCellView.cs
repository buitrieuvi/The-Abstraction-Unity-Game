using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lottery
{
    public class ScratchCellView : ViewBase, IPointerDownHandler
    {
        [Header("UI References")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image coverFoilImage;
        [SerializeField] private Image highlightGlow;
        [SerializeField] private TextMeshProUGUI txtCoverNumber;
        [SerializeField] private TextMeshProUGUI txtSymbolEmoji;
        [SerializeField] private TextMeshProUGUI txtSymbolName;
        [SerializeField] private TextMeshProUGUI txtPrize;

        [Header("Animation Settings")]
        [SerializeField] private float scratchDuration = 0.25f;

        private ScratchCellData cellData;
        private Action<int> onScratchAction;
        private Tween glowTween;

        public int CellIndex => cellData != null ? cellData.Index : -1;
        public bool IsScratched => cellData != null && cellData.IsScratched;

        public void Setup(ScratchCellData data, Action<int> onScratch)
        {
            cellData = data;
            onScratchAction = onScratch;

            if (glowTween != null)
            {
                glowTween.Kill();
                glowTween = null;
            }

            if (highlightGlow != null)
            {
                highlightGlow.gameObject.SetActive(false);
            }

            // Thiết lập nội dung ẩn bên dưới lớp bạc
            if (txtSymbolEmoji != null)
                txtSymbolEmoji.text = data.Emoji;

            if (txtSymbolName != null)
            {
                txtSymbolName.text = data.DisplayName;
                txtSymbolName.color = data.ThemeColor;
            }

            if (txtPrize != null)
                txtPrize.text = $"${data.PrizeValue:N0}";

            // Thiết lập lớp tráng bạc
            if (txtCoverNumber != null)
                txtCoverNumber.text = $"?";

            if (coverFoilImage != null)
            {
                coverFoilImage.DOKill();
                coverFoilImage.transform.localScale = Vector3.one;
                coverFoilImage.color = new Color(1f, 1f, 1f, 1f);
                coverFoilImage.gameObject.SetActive(!data.IsScratched);
            }

            if (data.IsScratched)
            {
                ApplyScratchedVisuals(instant: true);
            }
        }

        public void Scratch(bool instant = false)
        {
            if (cellData == null || cellData.IsScratched) return;

            cellData.IsScratched = true;
            onScratchAction?.Invoke(cellData.Index);
            ApplyScratchedVisuals(instant);
        }

        private void ApplyScratchedVisuals(bool instant)
        {
            if (coverFoilImage == null) return;

            if (instant)
            {
                coverFoilImage.gameObject.SetActive(false);
                return;
            }

            // Hiệu ứng cào vỡ lớp bạc bằng DOTween
            coverFoilImage.DOKill();
            coverFoilImage.transform.DOScale(0f, scratchDuration).SetEase(Ease.InBack);
            coverFoilImage.DOFade(0f, scratchDuration).OnComplete(() =>
            {
                coverFoilImage.gameObject.SetActive(false);
            });

            if (tfView != null)
            {
                tfView.DOKill();
                tfView.DOPunchScale(Vector3.one * 0.15f, 0.25f, 4, 0.5f);
            }
        }

        public void SetWinningHighlight(bool highlight)
        {
            if (highlightGlow == null) return;

            glowTween?.Kill();

            if (highlight)
            {
                highlightGlow.gameObject.SetActive(true);
                highlightGlow.transform.localScale = Vector3.one;
                glowTween = highlightGlow.transform.DOScale(1.15f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);

                if (bgImage != null && cellData != null)
                {
                    bgImage.color = cellData.ThemeColor;
                }
            }
            else
            {
                highlightGlow.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsScratched)
            {
                Scratch(false);
            }
        }

        public override void PointerEnter()
        {
            base.PointerEnter();

            // Cho phép rê chuột khi đang giữ chuột trái để cào liên tục
            if (Input.GetMouseButton(0) && !IsScratched)
            {
                Scratch(false);
            }
            else
            {
                Dotween_Ani_HoverEnter(1.05f);
            }
        }

        public override void PointerExit()
        {
            base.PointerExit();
            Dotween_Ani_HoverExit();
        }

        public override void PointerClick()
        {
            base.PointerClick();
            if (!IsScratched)
            {
                Scratch(false);
            }
        }

        private void OnDestroy()
        {
            glowTween?.Kill();
        }
    }
}
