using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Lottery
{
    public class ScratchTicketMachineView : ViewBase
    {
        [Header("Header & Player Money")]
        [SerializeField] private TextMeshProUGUI txtPlayerMoney;
        [SerializeField] private TextMeshProUGUI txtMachineTitle;
        [SerializeField] private TextMeshProUGUI txtStatusMessage;

        [Header("Tier Selection")]
        [SerializeField] private Button btnTierBronze;
        [SerializeField] private Button btnTierSilver;
        [SerializeField] private Button btnTierGold;
        [SerializeField] private Image imgTierBronzeBorder;
        [SerializeField] private Image imgTierSilverBorder;
        [SerializeField] private Image imgTierGoldBorder;

        [Header("Ticket Board")]
        [SerializeField] private GameObject ticketContainer;
        [SerializeField] private TextMeshProUGUI txtTicketTitle;
        [SerializeField] private TextMeshProUGUI txtTicketRule;
        [SerializeField] private Transform cellsGridContainer;
        [SerializeField] private ScratchCellView cellViewPrefab;

        [Header("Action Buttons")]
        [SerializeField] private Button btnBuyTicket;
        [SerializeField] private Button btnScratchAll;
        [SerializeField] private Button btnClaimReward;
        [SerializeField] private Button btnPlayAgain;
        [SerializeField] private Button btnClose;

        [Header("Result Banner")]
        [SerializeField] private GameObject resultBanner;
        [SerializeField] private TextMeshProUGUI txtResultTitle;
        [SerializeField] private TextMeshProUGUI txtResultReward;

        private ScratchTicketController controller;
        private ScratchTicketTier selectedTier = ScratchTicketTier.Bronze;
        private readonly List<ScratchCellView> cellViews = new();
        private float displayedMoney;

        public override void Awake()
        {
            base.Awake();

            // Đăng ký sự kiện các nút chọn hạng vé
            if (btnTierBronze != null) btnTierBronze.onClick.AddListener(() => SelectTier(ScratchTicketTier.Bronze));
            if (btnTierSilver != null) btnTierSilver.onClick.AddListener(() => SelectTier(ScratchTicketTier.Silver));
            if (btnTierGold != null) btnTierGold.onClick.AddListener(() => SelectTier(ScratchTicketTier.Gold));

            // Đăng ký các nút hành động
            if (btnBuyTicket != null) btnBuyTicket.onClick.AddListener(OnBuyTicketClicked);
            if (btnScratchAll != null) btnScratchAll.onClick.AddListener(OnScratchAllClicked);
            if (btnClaimReward != null) btnClaimReward.onClick.AddListener(OnClaimRewardClicked);
            if (btnPlayAgain != null) btnPlayAgain.onClick.AddListener(OnPlayAgainClicked);
            if (btnClose != null) btnClose.onClick.AddListener(OnCloseClicked);
        }

        public override void Open(ControllerBase ctrl)
        {
            base.Open(ctrl);

            controller = ctrl as ScratchTicketController;
            if (controller == null)
            {
                Debug.LogError("ScratchTicketMachineView yêu cầu ScratchTicketController!");
                return;
            }

            // Đăng ký lắng nghe sự kiện từ Controller
            controller.OnTicketPurchased += HandleTicketPurchased;
            controller.OnCellScratched += HandleCellScratched;
            controller.OnTicketCompleted += HandleTicketCompleted;
            controller.OnRewardClaimed += HandleRewardClaimed;
            controller.OnMoneyChanged += UpdateMoneyDisplay;

            // Khởi tạo hiển thị ban đầu
            if (dataManager != null)
            {
                displayedMoney = dataManager.Money;
                UpdateMoneyText(displayedMoney);
            }

            SelectTier(ScratchTicketTier.Bronze);

            if (resultBanner != null) resultBanner.SetActive(false);
            if (btnClaimReward != null) btnClaimReward.gameObject.SetActive(false);
            if (btnScratchAll != null) btnScratchAll.interactable = false;
            if (btnPlayAgain != null) btnPlayAgain.gameObject.SetActive(false);

            SetStatus("Chào mừng bạn đến với Máy Bán Vé Số Cào! Hãy chọn loại vé và bấm Mua Vé.");

            // Đóng băng di chuyển của nhân vật khi mở giao diện
            var player = Object.FindAnyObjectByType<PlayerController>();
            if (player != null && player.Movement != null)
            {
                player.Movement.ChangeState(player.Movement.freezing);
            }

            // Hiệu ứng mở popup lướt vào mượt mà
            if (tfView != null)
            {
                tfView.DOKill();
                tfView.localScale = Vector3.one * 0.85f;
                tfView.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
            }
        }

        public override void Close()
        {
            base.Close();

            if (controller != null)
            {
                controller.OnTicketPurchased -= HandleTicketPurchased;
                controller.OnCellScratched -= HandleCellScratched;
                controller.OnTicketCompleted -= HandleTicketCompleted;
                controller.OnRewardClaimed -= HandleRewardClaimed;
                controller.OnMoneyChanged -= UpdateMoneyDisplay;
            }

            // Khôi phục trạng thái di chuyển cho người chơi
            var player = Object.FindAnyObjectByType<PlayerController>();
            if (player != null && player.Movement != null)
            {
                player.Movement.ChangeState(player.Movement.idling);
            }
        }

        private void SelectTier(ScratchTicketTier tier)
        {
            selectedTier = tier;
            var config = controller != null ? controller.Model.GetConfig(tier) : null;
            float price = config != null ? config.Price : 50f;

            // Đổi hiệu ứng viền chọn
            if (imgTierBronzeBorder != null) imgTierBronzeBorder.color = tier == ScratchTicketTier.Bronze ? Color.yellow : Color.gray;
            if (imgTierSilverBorder != null) imgTierSilverBorder.color = tier == ScratchTicketTier.Silver ? Color.yellow : Color.gray;
            if (imgTierGoldBorder != null) imgTierGoldBorder.color = tier == ScratchTicketTier.Gold ? Color.yellow : Color.gray;

            bool canAfford = controller != null && controller.CanAffordTicket(tier);
            if (btnBuyTicket != null)
            {
                btnBuyTicket.interactable = canAfford;
                var txtBuy = btnBuyTicket.GetComponentInChildren<TextMeshProUGUI>();
                if (txtBuy != null)
                {
                    txtBuy.text = $"MUA VÉ (${price:N0})";
                }
            }

            SetStatus($"Đã chọn: {config?.TierName}. Giá: ${price:N0}. Tỷ lệ thắng: {(config != null ? config.WinRate * 100f : 40)}%.");
        }

        private void OnBuyTicketClicked()
        {
            if (controller == null) return;

            if (controller.BuyTicket(selectedTier, out string failureReason))
            {
                if (btnBuyTicket != null) btnBuyTicket.gameObject.SetActive(false);
                if (btnPlayAgain != null) btnPlayAgain.gameObject.SetActive(false);
                if (btnScratchAll != null)
                {
                    btnScratchAll.gameObject.SetActive(true);
                    btnScratchAll.interactable = true;
                }
                if (resultBanner != null) resultBanner.SetActive(false);
            }
            else
            {
                SetStatus($"<color=red>{failureReason}</color>");
                if (txtPlayerMoney != null)
                {
                    txtPlayerMoney.transform.DOPunchPosition(Vector3.right * 10f, 0.4f, 8);
                }
            }
        }

        private void HandleTicketPurchased(ScratchTicketData ticket)
        {
            SetStatus($"Đã mua {ticket.TierName}! Hãy dùng chuột cào các ô bên dưới (Cào trúng 3 hình giống nhau để nhận thưởng).");

            if (txtTicketTitle != null)
                txtTicketTitle.text = ticket.TierName.ToUpper();

            if (txtTicketRule != null)
                txtTicketRule.text = "CÀO TRÚNG 3 BIỂU TƯỢNG GIỐNG NHAU ĐỂ ĂN GIẢI";

            BuildCells(ticket);
        }

        private void BuildCells(ScratchTicketData ticket)
        {
            // Dọn dẹp các ô cũ
            foreach (var cell in cellViews)
            {
                if (cell != null) Destroy(cell.gameObject);
            }
            cellViews.Clear();

            if (cellsGridContainer == null || cellViewPrefab == null)
            {
                Debug.LogWarning("cellsGridContainer hoặc cellViewPrefab chưa được gán!");
                return;
            }

            // Tạo các ô cào mới
            for (int i = 0; i < ticket.Cells.Count; i++)
            {
                var cellData = ticket.Cells[i];
                var cellObj = Instantiate(cellViewPrefab, cellsGridContainer);
                cellObj.Setup(cellData, OnCellClicked);

                // Hoạt cảnh xuất hiện nảy từng ô
                cellObj.transform.localScale = Vector3.zero;
                cellObj.transform.DOScale(1f, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.03f);

                cellViews.Add(cellObj);
            }
        }

        private void OnCellClicked(int index)
        {
            if (controller != null)
            {
                controller.ScratchCell(index);
            }
        }

        private void HandleCellScratched(int index, ScratchCellData cell)
        {
            if (index >= 0 && index < cellViews.Count)
            {
                cellViews[index].Scratch(false);
            }

            var ticket = controller?.Model.CurrentTicket;
            if (ticket != null)
            {
                SetStatus($"Đã cào: {ticket.ScratchedCount}/{ticket.Cells.Count} ô...");
            }
        }

        private void OnScratchAllClicked()
        {
            if (controller == null) return;
            if (btnScratchAll != null) btnScratchAll.interactable = false;

            controller.ScratchAll();
        }

        private void HandleTicketCompleted(ScratchTicketData ticket)
        {
            if (btnScratchAll != null) btnScratchAll.gameObject.SetActive(false);

            if (ticket.IsWinner)
            {
                // Làm nổi bật các ô trúng thưởng
                for (int i = 0; i < ticket.Cells.Count; i++)
                {
                    if (ticket.Cells[i].IsPartOfWinningCombo && i < cellViews.Count)
                    {
                        cellViews[i].SetWinningHighlight(true);
                    }
                }

                SetStatus($"<color=#FFD700><b>🎉 CHÚC MỪNG! {ticket.WinReason.ToUpper()} 🎉</b></color>");

                if (resultBanner != null)
                {
                    resultBanner.SetActive(true);
                    resultBanner.transform.localScale = Vector3.zero;
                    resultBanner.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
                }

                if (txtResultTitle != null) txtResultTitle.text = "🎉 BẠN ĐÃ TRÚNG THƯỞNG! 🎉";
                if (txtResultReward != null) txtResultReward.text = $"+${ticket.TotalRewardMoney:N0}";

                if (btnClaimReward != null)
                {
                    btnClaimReward.gameObject.SetActive(true);
                    btnClaimReward.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 5, 0.5f);
                }
            }
            else
            {
                SetStatus("<color=#A0A0A0>Vé không trúng. Đừng nản lòng, thần may mắn sẽ mỉm cười lần sau!</color>");

                if (resultBanner != null)
                {
                    resultBanner.SetActive(true);
                    resultBanner.transform.localScale = Vector3.zero;
                    resultBanner.transform.DOScale(1f, 0.35f);
                }

                if (txtResultTitle != null) txtResultTitle.text = "CHÚC MAY MẮN LẦN SAU!";
                if (txtResultReward != null) txtResultReward.text = "$0";

                if (btnPlayAgain != null) btnPlayAgain.gameObject.SetActive(true);
                if (btnBuyTicket != null) btnBuyTicket.gameObject.SetActive(true);
            }
        }

        private void OnClaimRewardClicked()
        {
            if (controller == null) return;
            controller.ClaimReward(out _, out _);
        }

        private void HandleRewardClaimed(float rewardMoney, string rewardItemId)
        {
            if (btnClaimReward != null) btnClaimReward.gameObject.SetActive(false);
            if (btnPlayAgain != null) btnPlayAgain.gameObject.SetActive(true);
            if (btnBuyTicket != null) btnBuyTicket.gameObject.SetActive(true);

            string rewardMsg = $"Đã nhận thưởng ${rewardMoney:N0} vào tài khoản!";
            if (!string.IsNullOrEmpty(rewardItemId))
            {
                rewardMsg += $" Kèm vật phẩm ({rewardItemId}) đã chuyển vào Túi Đồ!";
            }
            SetStatus($"<color=#00FF88>{rewardMsg}</color>");
        }

        private void OnPlayAgainClicked()
        {
            if (resultBanner != null) resultBanner.SetActive(false);
            if (btnPlayAgain != null) btnPlayAgain.gameObject.SetActive(false);
            if (btnBuyTicket != null) btnBuyTicket.gameObject.SetActive(true);

            // Dọn các ô cào
            foreach (var cell in cellViews)
            {
                if (cell != null) Destroy(cell.gameObject);
            }
            cellViews.Clear();

            SelectTier(selectedTier);
        }

        private async void OnCloseClicked()
        {
            if (viewManager != null)
            {
                await viewManager.ShowView<ScratchTicketMachineView>(controller);
            }
            else
            {
                Close();
            }
        }

        private void UpdateMoneyDisplay(float newMoney)
        {
            if (txtPlayerMoney == null) return;

            float startVal = displayedMoney;
            DOTween.To(() => startVal, x =>
            {
                displayedMoney = x;
                UpdateMoneyText(displayedMoney);
            }, newMoney, 0.6f).SetEase(Ease.OutQuad);
        }

        private void UpdateMoneyText(float money)
        {
            if (txtPlayerMoney != null)
            {
                txtPlayerMoney.text = $"${money:N0}";
            }
        }

        private void SetStatus(string message)
        {
            if (txtStatusMessage != null)
            {
                txtStatusMessage.text = message;
            }
        }
    }
}
