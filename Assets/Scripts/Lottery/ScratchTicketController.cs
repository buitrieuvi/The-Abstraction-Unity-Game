using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lottery
{
    [Serializable]
    public class ScratchTicketController : ControllerBase
    {
        public ScratchTicketModel Model { get; private set; }
        public DataManager DataManager { get; set; }

        public event Action<ScratchTicketData> OnTicketPurchased;
        public event Action<int, ScratchCellData> OnCellScratched;
        public event Action<ScratchTicketData> OnTicketCompleted;
        public event Action<float, string> OnRewardClaimed;
        public event Action<float> OnMoneyChanged;

        private static readonly Dictionary<ScratchSymbolType, ScratchSymbolDefinition> SymbolDefinitions = new()
        {
            { ScratchSymbolType.Coin, new ScratchSymbolDefinition(ScratchSymbolType.Coin, "Đồng Vàng", "🪙", new Color(1.0f, 0.85f, 0.2f), 1.5f) },
            { ScratchSymbolType.Cash, new ScratchSymbolDefinition(ScratchSymbolType.Cash, "Cọc Tiền", "💵", new Color(0.2f, 0.85f, 0.3f), 2.5f) },
            { ScratchSymbolType.Clover, new ScratchSymbolDefinition(ScratchSymbolType.Clover, "Cỏ 4 Lá", "🍀", new Color(0.1f, 0.9f, 0.4f), 5.0f) },
            { ScratchSymbolType.Star, new ScratchSymbolDefinition(ScratchSymbolType.Star, "Ngôi Sao", "⭐", new Color(1.0f, 0.65f, 0.0f), 10.0f) },
            { ScratchSymbolType.Crown, new ScratchSymbolDefinition(ScratchSymbolType.Crown, "Vương Miện", "👑", new Color(0.95f, 0.4f, 0.9f), 25.0f) },
            { ScratchSymbolType.Gem, new ScratchSymbolDefinition(ScratchSymbolType.Gem, "Kim Cương", "💎", new Color(0.2f, 0.8f, 1.0f), 50.0f) },
            { ScratchSymbolType.Jackpot7, new ScratchSymbolDefinition(ScratchSymbolType.Jackpot7, "Số 7 May Mắn", "7️⃣", new Color(1.0f, 0.2f, 0.2f), 100.0f) }
        };

        public ScratchTicketController(DataManager dataManager, ScratchTicketModel model = null)
        {
            DataManager = dataManager;
            Model = model ?? new ScratchTicketModel();
        }

        public bool CanAffordTicket(ScratchTicketTier tier)
        {
            if (DataManager == null) return false;
            var config = Model.GetConfig(tier);
            return DataManager.Money >= config.Price;
        }

        public bool BuyTicket(ScratchTicketTier tier, out string failureReason)
        {
            failureReason = string.Empty;

            if (DataManager == null)
            {
                failureReason = "Hệ thống dữ liệu chưa sẵn sàng!";
                return false;
            }

            var config = Model.GetConfig(tier);

            if (DataManager.Money < config.Price)
            {
                failureReason = $"Không đủ tiền! Bạn cần có ít nhất ${config.Price:N0} để mua vé này.";
                return false;
            }

            // Trừ tiền mua vé
            DataManager.Money -= config.Price;
            Model.TotalTicketsBought++;
            Model.TotalMoneySpent += config.Price;

            // Tạo vé mới ngẫu nhiên
            var ticket = GenerateTicket(config);
            Model.CurrentTicket = ticket;

            OnTicketPurchased?.Invoke(ticket);
            OnMoneyChanged?.Invoke(DataManager.Money);

            return true;
        }

        public bool ScratchCell(int cellIndex)
        {
            var ticket = Model.CurrentTicket;
            if (ticket == null || cellIndex < 0 || cellIndex >= ticket.Cells.Count)
                return false;

            var cell = ticket.Cells[cellIndex];
            if (cell.IsScratched)
                return false;

            cell.IsScratched = true;
            OnCellScratched?.Invoke(cellIndex, cell);

            if (ticket.ScratchedCount >= ticket.Cells.Count)
            {
                ticket.IsAllScratched = true;
                OnTicketCompleted?.Invoke(ticket);
            }

            return true;
        }

        public void ScratchAll()
        {
            var ticket = Model.CurrentTicket;
            if (ticket == null || ticket.IsAllScratched) return;

            for (int i = 0; i < ticket.Cells.Count; i++)
            {
                if (!ticket.Cells[i].IsScratched)
                {
                    ticket.Cells[i].IsScratched = true;
                    OnCellScratched?.Invoke(i, ticket.Cells[i]);
                }
            }

            ticket.IsAllScratched = true;
            OnTicketCompleted?.Invoke(ticket);
        }

        public bool ClaimReward(out float rewardMoney, out string rewardItemId)
        {
            rewardMoney = 0f;
            rewardItemId = null;

            var ticket = Model.CurrentTicket;
            if (ticket == null || !ticket.IsWinner || ticket.IsRewardClaimed)
                return false;

            ticket.IsRewardClaimed = true;
            rewardMoney = ticket.TotalRewardMoney;
            rewardItemId = ticket.RewardItemId;

            if (DataManager != null)
            {
                DataManager.Money += rewardMoney;

                if (!string.IsNullOrEmpty(rewardItemId) && DataManager.PlayerInventory != null)
                {
                    DataManager.PlayerInventory.ChangedItem(rewardItemId, 1);
                }
            }

            Model.TotalWins++;
            Model.TotalMoneyWon += rewardMoney;

            OnRewardClaimed?.Invoke(rewardMoney, rewardItemId);
            OnMoneyChanged?.Invoke(DataManager != null ? DataManager.Money : 0f);

            return true;
        }

        private ScratchTicketData GenerateTicket(ScratchTicketConfig config)
        {
            var ticket = new ScratchTicketData
            {
                Tier = config.Tier,
                TierName = config.TierName,
                Price = config.Price,
                Cells = new List<ScratchCellData>(config.TotalCells)
            };

            bool isWin = UnityEngine.Random.value <= config.WinRate;
            ScratchSymbolType? winSymbol = null;

            if (isWin)
            {
                winSymbol = PickWinningSymbol(config.Tier);
            }

            // Tạo các ô cào với thuật toán Match-3
            List<ScratchSymbolType> cellSymbols = new List<ScratchSymbolType>(config.TotalCells);

            if (isWin && winSymbol.HasValue)
            {
                // Thêm 3 biểu tượng trúng giải
                for (int i = 0; i < config.RequiredMatchCount; i++)
                {
                    cellSymbols.Add(winSymbol.Value);
                }

                // Điền các ô còn lại sao cho không trùng 3 lần biểu tượng nào khác
                Dictionary<ScratchSymbolType, int> counts = new() { { winSymbol.Value, 3 } };
                List<ScratchSymbolType> availableTypes = Enum.GetValues(typeof(ScratchSymbolType))
                    .Cast<ScratchSymbolType>()
                    .Where(t => t != winSymbol.Value)
                    .ToList();

                while (cellSymbols.Count < config.TotalCells)
                {
                    var randomType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
                    counts.TryGetValue(randomType, out int c);
                    if (c < 2) // Giữ tối đa 2 để không tạo thêm combo thắng ngẫu nhiên
                    {
                        cellSymbols.Add(randomType);
                        counts[randomType] = c + 1;
                    }
                }
            }
            else
            {
                // Vé thua: Đảm bảo không có bất kỳ biểu tượng nào xuất hiện >= 3 lần
                Dictionary<ScratchSymbolType, int> counts = new();
                List<ScratchSymbolType> allTypes = Enum.GetValues(typeof(ScratchSymbolType)).Cast<ScratchSymbolType>().ToList();

                while (cellSymbols.Count < config.TotalCells)
                {
                    var randomType = allTypes[UnityEngine.Random.Range(0, allTypes.Count)];
                    counts.TryGetValue(randomType, out int c);
                    if (c < 2)
                    {
                        cellSymbols.Add(randomType);
                        counts[randomType] = c + 1;
                    }
                }
            }

            // Xáo trộn vị trí các ô
            cellSymbols = cellSymbols.OrderBy(_ => UnityEngine.Random.value).ToList();

            // Khởi tạo danh sách ScratchCellData
            for (int i = 0; i < cellSymbols.Count; i++)
            {
                var sType = cellSymbols[i];
                var def = SymbolDefinitions[sType];
                bool isWinningCombo = isWin && winSymbol.HasValue && sType == winSymbol.Value;

                ticket.Cells.Add(new ScratchCellData
                {
                    Index = i,
                    SymbolType = sType,
                    DisplayName = def.DisplayName,
                    Emoji = def.Emoji,
                    ThemeColor = def.ThemeColor,
                    PrizeValue = config.Price * def.Multiplier,
                    RewardItemId = def.SpecialItemId,
                    IsScratched = false,
                    IsPartOfWinningCombo = isWinningCombo
                });
            }

            ticket.IsWinner = isWin;
            ticket.WinningSymbol = winSymbol;

            if (isWin && winSymbol.HasValue)
            {
                var winDef = SymbolDefinitions[winSymbol.Value];
                ticket.TotalRewardMoney = config.Price * winDef.Multiplier;
                ticket.RewardItemId = winDef.SpecialItemId;
                ticket.WinReason = $"Trúng 3x {winDef.Emoji} {winDef.DisplayName}!";
            }
            else
            {
                ticket.TotalRewardMoney = 0f;
                ticket.WinReason = "Chúc bạn may mắn lần sau!";
            }

            return ticket;
        }

        private ScratchSymbolType PickWinningSymbol(ScratchTicketTier tier)
        {
            // Tỷ lệ trúng các biểu tượng theo từng hạng vé
            float r = UnityEngine.Random.value;

            switch (tier)
            {
                case ScratchTicketTier.Bronze:
                    if (r < 0.50f) return ScratchSymbolType.Coin;      // 1.5x ($75)
                    if (r < 0.80f) return ScratchSymbolType.Cash;      // 2.5x ($125)
                    if (r < 0.95f) return ScratchSymbolType.Clover;    // 5.0x ($250)
                    return ScratchSymbolType.Star;                     // 10.0x ($500)

                case ScratchTicketTier.Silver:
                    if (r < 0.40f) return ScratchSymbolType.Cash;      // 2.5x ($500)
                    if (r < 0.70f) return ScratchSymbolType.Clover;    // 5.0x ($1,000)
                    if (r < 0.88f) return ScratchSymbolType.Star;      // 10.0x ($2,000)
                    if (r < 0.97f) return ScratchSymbolType.Crown;     // 25.0x ($5,000)
                    return ScratchSymbolType.Gem;                      // 50.0x ($10,000)

                case ScratchTicketTier.Gold:
                default:
                    if (r < 0.35f) return ScratchSymbolType.Clover;    // 5.0x ($2,500)
                    if (r < 0.65f) return ScratchSymbolType.Star;      // 10.0x ($5,000)
                    if (r < 0.85f) return ScratchSymbolType.Crown;     // 25.0x ($12,500)
                    if (r < 0.96f) return ScratchSymbolType.Gem;       // 50.0x ($25,000)
                    return ScratchSymbolType.Jackpot7;                 // 100.0x ($50,000 JACKPOT!)
            }
        }
    }
}

