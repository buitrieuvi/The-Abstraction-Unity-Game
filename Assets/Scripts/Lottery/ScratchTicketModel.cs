using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lottery
{
    public enum ScratchTicketTier
    {
        Bronze = 0, // Vé Đồng - $50
        Silver = 1, // Vé Bạc - $200
        Gold = 2    // Vé Vàng - $500
    }

    public enum ScratchSymbolType
    {
        Coin = 0,     // Đồng Xu Vàng
        Cash = 1,     // Cọc Tiền
        Clover = 2,   // Cỏ 4 Lá
        Star = 3,     // Ngôi Sao
        Crown = 4,    // Vương Miện
        Gem = 5,      // Kim Cương
        Jackpot7 = 6  // Số 7 May Mắn (Jackpot)
    }

    [Serializable]
    public class ScratchSymbolDefinition
    {
        public ScratchSymbolType Type;
        public string DisplayName;
        public string Emoji;
        public Color ThemeColor;
        public float Multiplier; // Hệ số nhân giá vé làm tiền thưởng
        public string SpecialItemId; // Item thưởng kèm (nếu có)

        public ScratchSymbolDefinition(ScratchSymbolType type, string displayName, string emoji, Color color, float multiplier, string specialItemId = null)
        {
            Type = type;
            DisplayName = displayName;
            Emoji = emoji;
            ThemeColor = color;
            Multiplier = multiplier;
            SpecialItemId = specialItemId;
        }
    }

    [Serializable]
    public class ScratchCellData
    {
        public int Index;
        public ScratchSymbolType SymbolType;
        public string DisplayName;
        public string Emoji;
        public Color ThemeColor;
        public float PrizeValue;
        public string RewardItemId;
        public bool IsScratched;
        public bool IsPartOfWinningCombo;
    }   

    [Serializable]
    public class ScratchTicketConfig
    {
        public ScratchTicketTier Tier;
        public string TierName;
        public float Price;
        [Range(0f, 1f)] public float WinRate = 0.40f;
        public int TotalCells = 9;
        public int RequiredMatchCount = 3;
        public Color TierColor = Color.white;
    }

    [Serializable]
    public class ScratchTicketData
    {
        public ScratchTicketTier Tier;
        public string TierName;
        public float Price;
        public List<ScratchCellData> Cells = new();
        public bool IsWinner;
        [NonSerialized] public ScratchSymbolType? WinningSymbol;
        public float TotalRewardMoney;
        public string RewardItemId;
        public string WinReason;
        public bool IsAllScratched;
        public bool IsRewardClaimed;

        public int ScratchedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (Cells[i].IsScratched) count++;
                }
                return count;
            }
        }
    }

    [Serializable]
    public class ScratchTicketModel : BaseModel
    {
        public List<ScratchTicketConfig> Configs = new();
        public ScratchTicketData CurrentTicket;

        // Thống kê phiên chơi
        public int TotalTicketsBought;
        public float TotalMoneySpent;
        public float TotalMoneyWon;
        public int TotalWins;

        public ScratchTicketModel()
        {
            InitDefaultConfigs();
        }

        public void InitDefaultConfigs()
        {
            Configs = new List<ScratchTicketConfig>
            {
                new ScratchTicketConfig
                {
                    Tier = ScratchTicketTier.Bronze,
                    TierName = "Vé Cào Đồng (Bronze)",
                    Price = 50f,
                    WinRate = 0.45f,
                    TotalCells = 9,
                    RequiredMatchCount = 3,
                    TierColor = new Color(0.80f, 0.50f, 0.20f)
                },
                new ScratchTicketConfig
                {
                    Tier = ScratchTicketTier.Silver,
                    TierName = "Vé Cào Bạc (Silver)",
                    Price = 200f,
                    WinRate = 0.40f,
                    TotalCells = 9,
                    RequiredMatchCount = 3,
                    TierColor = new Color(0.75f, 0.85f, 0.95f)
                },
                new ScratchTicketConfig
                {
                    Tier = ScratchTicketTier.Gold,
                    TierName = "Vé Cào Vàng (Gold VIP)",
                    Price = 500f,
                    WinRate = 0.35f,
                    TotalCells = 9,
                    RequiredMatchCount = 3,
                    TierColor = new Color(1.0f, 0.84f, 0.0f)
                }
            };
        }

        public ScratchTicketConfig GetConfig(ScratchTicketTier tier)
        {
            return Configs.Find(c => c.Tier == tier) ?? Configs[0];
        }
    }
}
