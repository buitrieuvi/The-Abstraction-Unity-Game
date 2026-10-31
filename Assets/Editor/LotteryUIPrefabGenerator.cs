using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Lottery.Editor
{
    [InitializeOnLoad]
    public static class LotteryUIPrefabGenerator
    {
        private const string PrefabDir = "Assets/Prefabs/Lottery";
        private const string CellPrefabPath = "Assets/Prefabs/Lottery/ScratchCellView.prefab";
        private const string MachineViewPrefabPath = "Assets/Prefabs/Lottery/ScratchTicketMachineView.prefab";
        private const string FontGuid = "8f586378b4e144a9851e7b34d9b748ee";

        static LotteryUIPrefabGenerator()
        {
            EditorApplication.delayCall += CheckAndGenerateUIPrefabs;
        }

        [MenuItem("Lottery/Generate UI Prefabs & Connect")]
        public static void ForceGenerateUIPrefabs()
        {
            GeneratePrefabs(true);
        }

        public static void CheckAndGenerateUIPrefabs()
        {
            if (!File.Exists(CellPrefabPath) || !File.Exists(MachineViewPrefabPath))
            {
                GeneratePrefabs(false);
            }
        }

        private static void GeneratePrefabs(bool force)
        {
            if (!Directory.Exists(PrefabDir))
            {
                Directory.CreateDirectory(PrefabDir);
            }

            TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(FontGuid));

            // 1. Tạo ScratchCellView Prefab
            GameObject cellObj = CreateCellGameObject(fontAsset);
            GameObject cellPrefab = PrefabUtility.SaveAsPrefabAsset(cellObj, CellPrefabPath);
            Object.DestroyImmediate(cellObj);

            // 2. Tạo ScratchTicketMachineView Prefab
            ScratchCellView cellViewComponent = cellPrefab.GetComponent<ScratchCellView>();
            GameObject viewObj = CreateMachineViewGameObject(fontAsset, cellViewComponent);
            GameObject viewPrefab = PrefabUtility.SaveAsPrefabAsset(viewObj, MachineViewPrefabPath);
            Object.DestroyImmediate(viewObj);

            // 3. Đăng ký vào Addressables với label "view"
            RegisterViewInAddressables(MachineViewPrefabPath);

            // 4. Gán fallbackViewPrefab vào Lottery Machine Prefab & Scene
            ConnectViewToMachine(viewPrefab.GetComponent<ScratchTicketMachineView>());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=#00FF88>[Lottery] Đã tạo thành công UI Prefabs cho ScratchTicketMachineView & ScratchCellView!</color>");
        }

        private static GameObject CreateCellGameObject(TMP_FontAsset font)
        {
            GameObject root = new GameObject("ScratchCellView", typeof(RectTransform));
            RectTransform rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(125f, 125f);

            // Nền ô cào (Background)
            GameObject bgObj = CreateUIElement("Bg", root.transform);
            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.18f, 0.15f, 0.28f, 1f); // Tím sẫm cao cấp

            // Viền phát sáng khi trúng giải (Highlight Glow)
            GameObject glowObj = CreateUIElement("HighlightGlow", root.transform);
            RectTransform glowRt = glowObj.GetComponent<RectTransform>();
            glowRt.anchorMin = Vector2.zero;
            glowRt.anchorMax = Vector2.one;
            glowRt.sizeDelta = new Vector2(10f, 10f);
            Image glowImg = glowObj.AddComponent<Image>();
            glowImg.color = new Color(1f, 0.84f, 0f, 0.85f); // Vàng kim rực rỡ
            glowObj.SetActive(false);

            // Reveal Content Container (Nội dung ẩn bên dưới lớp bạc)
            GameObject contentObj = CreateUIElement("RevealContent", root.transform);
            RectTransform contentRt = contentObj.GetComponent<RectTransform>();
            contentRt.anchorMin = Vector2.zero;
            contentRt.anchorMax = Vector2.one;
            contentRt.sizeDelta = Vector2.zero;

            // Biểu tượng Emoji
            GameObject emojiObj = CreateUIElement("TxtEmoji", contentObj.transform);
            RectTransform emojiRt = emojiObj.GetComponent<RectTransform>();
            emojiRt.anchorMin = new Vector2(0f, 0.35f);
            emojiRt.anchorMax = new Vector2(1f, 0.95f);
            emojiRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtEmoji = emojiObj.AddComponent<TextMeshProUGUI>();
            txtEmoji.font = font;
            txtEmoji.fontSize = 44f;
            txtEmoji.alignment = TextAlignmentOptions.Center;
            txtEmoji.text = "💎";

            // Tên biểu tượng
            GameObject nameObj = CreateUIElement("TxtName", contentObj.transform);
            RectTransform nameRt = nameObj.GetComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0f, 0.22f);
            nameRt.anchorMax = new Vector2(1f, 0.42f);
            nameRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtName = nameObj.AddComponent<TextMeshProUGUI>();
            txtName.font = font;
            txtName.fontSize = 13f;
            txtName.fontStyle = FontStyles.Bold;
            txtName.alignment = TextAlignmentOptions.Center;
            txtName.text = "KIM CƯƠNG";

            // Giá trị giải thưởng
            GameObject prizeObj = CreateUIElement("TxtPrize", contentObj.transform);
            RectTransform prizeRt = prizeObj.GetComponent<RectTransform>();
            prizeRt.anchorMin = new Vector2(0f, 0.04f);
            prizeRt.anchorMax = new Vector2(1f, 0.24f);
            prizeRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtPrize = prizeObj.AddComponent<TextMeshProUGUI>();
            txtPrize.font = font;
            txtPrize.fontSize = 14f;
            txtPrize.fontStyle = FontStyles.Bold;
            txtPrize.color = new Color(1f, 0.85f, 0.2f);
            txtPrize.alignment = TextAlignmentOptions.Center;
            txtPrize.text = "$1,500";

            // Lớp tráng bạc (Cover Foil)
            GameObject foilObj = CreateUIElement("CoverFoil", root.transform);
            RectTransform foilRt = foilObj.GetComponent<RectTransform>();
            foilRt.anchorMin = Vector2.zero;
            foilRt.anchorMax = Vector2.one;
            foilRt.sizeDelta = Vector2.zero;
            Image foilImg = foilObj.AddComponent<Image>();
            foilImg.color = new Color(0.78f, 0.82f, 0.88f, 1f); // Màu bạc nhũ

            // Dấu hỏi hoặc số trên lớp bạc
            GameObject coverTextObj = CreateUIElement("TxtCover", foilObj.transform);
            RectTransform coverTextRt = coverTextObj.GetComponent<RectTransform>();
            coverTextRt.anchorMin = Vector2.zero;
            coverTextRt.anchorMax = Vector2.one;
            coverTextRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtCover = coverTextObj.AddComponent<TextMeshProUGUI>();
            txtCover.font = font;
            txtCover.fontSize = 42f;
            txtCover.fontStyle = FontStyles.Bold;
            txtCover.color = new Color(0.35f, 0.4f, 0.5f, 1f);
            txtCover.alignment = TextAlignmentOptions.Center;
            txtCover.text = "?";

            // Gán component ScratchCellView
            ScratchCellView cellView = root.AddComponent<ScratchCellView>();
            SerializedObject so = new SerializedObject(cellView);
            so.FindProperty("bgImage").objectReferenceValue = bgImg;
            so.FindProperty("coverFoilImage").objectReferenceValue = foilImg;
            so.FindProperty("highlightGlow").objectReferenceValue = glowImg;
            so.FindProperty("txtCoverNumber").objectReferenceValue = txtCover;
            so.FindProperty("txtSymbolEmoji").objectReferenceValue = txtEmoji;
            so.FindProperty("txtSymbolName").objectReferenceValue = txtName;
            so.FindProperty("txtPrize").objectReferenceValue = txtPrize;
            so.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static GameObject CreateMachineViewGameObject(TMP_FontAsset font, ScratchCellView cellPrefab)
        {
            GameObject root = new GameObject("ScratchTicketMachineView", typeof(RectTransform));
            RectTransform rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            // Màn che tối nền (Backdrop)
            GameObject backdropObj = CreateUIElement("Backdrop", root.transform);
            RectTransform bdRt = backdropObj.GetComponent<RectTransform>();
            bdRt.anchorMin = Vector2.zero;
            bdRt.anchorMax = Vector2.one;
            bdRt.sizeDelta = Vector2.zero;
            Image bdImg = backdropObj.AddComponent<Image>();
            bdImg.color = new Color(0f, 0f, 0f, 0.75f);

            // Main Modal Window (Khung cửa sổ chính)
            GameObject window = CreateUIElement("WindowPanel", root.transform);
            RectTransform winRt = window.GetComponent<RectTransform>();
            winRt.anchorMin = new Vector2(0.5f, 0.5f);
            winRt.anchorMax = new Vector2(0.5f, 0.5f);
            winRt.sizeDelta = new Vector2(980f, 680f);
            Image winImg = window.AddComponent<Image>();
            winImg.color = new Color(0.12f, 0.09f, 0.18f, 0.98f); // Tím đen hoàng gia

            // Viền vàng ngoài của cửa sổ
            GameObject winBorder = CreateUIElement("Border", window.transform);
            RectTransform wbRt = winBorder.GetComponent<RectTransform>();
            wbRt.anchorMin = Vector2.zero;
            wbRt.anchorMax = Vector2.one;
            wbRt.sizeDelta = new Vector2(6f, 6f);
            Image wbImg = winBorder.AddComponent<Image>();
            wbImg.color = new Color(0.85f, 0.72f, 0.35f, 1f); // Viền vàng kim
            winBorder.transform.SetAsFirstSibling();

            // --- HEADER BAR ---
            GameObject header = CreateUIElement("HeaderBar", window.transform);
            RectTransform hRt = header.GetComponent<RectTransform>();
            hRt.anchorMin = new Vector2(0f, 0.88f);
            hRt.anchorMax = new Vector2(1f, 1f);
            hRt.sizeDelta = Vector2.zero;
            Image hImg = header.AddComponent<Image>();
            hImg.color = new Color(0.2f, 0.15f, 0.3f, 1f);

            // Tiêu đề máy
            GameObject titleObj = CreateUIElement("TxtMachineTitle", header.transform);
            RectTransform tRt = titleObj.GetComponent<RectTransform>();
            tRt.anchorMin = new Vector2(0.04f, 0f);
            tRt.anchorMax = new Vector2(0.6f, 1f);
            tRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtTitle = titleObj.AddComponent<TextMeshProUGUI>();
            txtTitle.font = font;
            txtTitle.fontSize = 24f;
            txtTitle.fontStyle = FontStyles.Bold;
            txtTitle.color = new Color(1f, 0.84f, 0.0f);
            txtTitle.alignment = TextAlignmentOptions.MidlineLeft;
            txtTitle.text = "🎰 VÉ SỐ CÀO MAY MẮN (LUCKY LOTTERY)";

            // Số dư tiền người chơi
            GameObject moneyObj = CreateUIElement("TxtPlayerMoney", header.transform);
            RectTransform mRt = moneyObj.GetComponent<RectTransform>();
            mRt.anchorMin = new Vector2(0.6f, 0f);
            mRt.anchorMax = new Vector2(0.9f, 1f);
            mRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtMoney = moneyObj.AddComponent<TextMeshProUGUI>();
            txtMoney.font = font;
            txtMoney.fontSize = 22f;
            txtMoney.fontStyle = FontStyles.Bold;
            txtMoney.color = new Color(0.2f, 1f, 0.4f); // Xanh lá tiền tệ
            txtMoney.alignment = TextAlignmentOptions.MidlineRight;
            txtMoney.text = "$0";

            // Nút đóng (Close Button)
            GameObject closeBtnObj = CreateButton("BtnClose", header.transform, new Vector2(0.92f, 0.15f), new Vector2(0.98f, 0.85f), "✕", new Color(0.8f, 0.2f, 0.2f), font, 20f);
            Button btnClose = closeBtnObj.GetComponent<Button>();

            // --- STATUS MESSAGE BAR ---
            GameObject statusObj = CreateUIElement("TxtStatusMessage", window.transform);
            RectTransform stRt = statusObj.GetComponent<RectTransform>();
            stRt.anchorMin = new Vector2(0.05f, 0.81f);
            stRt.anchorMax = new Vector2(0.95f, 0.87f);
            stRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtStatus = statusObj.AddComponent<TextMeshProUGUI>();
            txtStatus.font = font;
            txtStatus.fontSize = 15f;
            txtStatus.alignment = TextAlignmentOptions.Center;
            txtStatus.text = "Chào mừng bạn! Hãy chọn loại vé và bấm MUA VÉ.";

            // --- LEFT PANEL: TIER SELECTION (Chọn gói vé) ---
            GameObject leftPanel = CreateUIElement("TierSelectionPanel", window.transform);
            RectTransform lpRt = leftPanel.GetComponent<RectTransform>();
            lpRt.anchorMin = new Vector2(0.04f, 0.12f);
            lpRt.anchorMax = new Vector2(0.38f, 0.80f);
            lpRt.sizeDelta = Vector2.zero;

            // Nút Vé Đồng ($50)
            GameObject btnBronzeObj = CreateButton("BtnBronze", leftPanel.transform, new Vector2(0f, 0.68f), new Vector2(1f, 0.98f), "🥉 VÉ ĐỒNG\n<size=75%>Giá: $50 • Thưởng max: $500</size>", new Color(0.45f, 0.28f, 0.15f), font, 16f);
            Button btnBronze = btnBronzeObj.GetComponent<Button>();
            Image imgBronzeBorder = btnBronzeObj.GetComponent<Image>();

            // Nút Vé Bạc ($200)
            GameObject btnSilverObj = CreateButton("BtnSilver", leftPanel.transform, new Vector2(0f, 0.35f), new Vector2(1f, 0.65f), "🥈 VÉ BẠC\n<size=75%>Giá: $200 • Thưởng max: $10,000</size>", new Color(0.3f, 0.38f, 0.48f), font, 16f);
            Button btnSilver = btnSilverObj.GetComponent<Button>();
            Image imgSilverBorder = btnSilverObj.GetComponent<Image>();

            // Nút Vé Vàng ($500)
            GameObject btnGoldObj = CreateButton("BtnGold", leftPanel.transform, new Vector2(0f, 0.02f), new Vector2(1f, 0.32f), "🥇 VÉ VÀNG VIP\n<size=75%>Giá: $500 • JACKPOT: $50,000</size>", new Color(0.6f, 0.48f, 0.1f), font, 16f);
            Button btnGold = btnGoldObj.GetComponent<Button>();
            Image imgGoldBorder = btnGoldObj.GetComponent<Image>();

            // --- RIGHT PANEL: TICKET BOARD (Khu vực cào vé) ---
            GameObject ticketBoard = CreateUIElement("TicketBoard", window.transform);
            RectTransform tbRt = ticketBoard.GetComponent<RectTransform>();
            tbRt.anchorMin = new Vector2(0.41f, 0.12f);
            tbRt.anchorMax = new Vector2(0.96f, 0.80f);
            tbRt.sizeDelta = Vector2.zero;
            Image tbImg = ticketBoard.AddComponent<Image>();
            tbImg.color = new Color(0.16f, 0.13f, 0.24f, 1f);

            // Tiêu đề vé
            GameObject ticketTitleObj = CreateUIElement("TxtTicketTitle", ticketBoard.transform);
            RectTransform ttRt = ticketTitleObj.GetComponent<RectTransform>();
            ttRt.anchorMin = new Vector2(0.05f, 0.88f);
            ttRt.anchorMax = new Vector2(0.95f, 0.98f);
            ttRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtTicketTitle = ticketTitleObj.AddComponent<TextMeshProUGUI>();
            txtTicketTitle.font = font;
            txtTicketTitle.fontSize = 20f;
            txtTicketTitle.fontStyle = FontStyles.Bold;
            txtTicketTitle.color = Color.white;
            txtTicketTitle.alignment = TextAlignmentOptions.Center;
            txtTicketTitle.text = "VÉ CÀO ĐỒNG";

            // Luật chơi
            GameObject ticketRuleObj = CreateUIElement("TxtTicketRule", ticketBoard.transform);
            RectTransform trRt = ticketRuleObj.GetComponent<RectTransform>();
            trRt.anchorMin = new Vector2(0.05f, 0.80f);
            trRt.anchorMax = new Vector2(0.95f, 0.88f);
            trRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtTicketRule = ticketRuleObj.AddComponent<TextMeshProUGUI>();
            txtTicketRule.font = font;
            txtTicketRule.fontSize = 13f;
            txtTicketRule.color = new Color(0.8f, 0.8f, 0.8f);
            txtTicketRule.alignment = TextAlignmentOptions.Center;
            txtTicketRule.text = "CÀO TRÚNG 3 HÌNH GIỐNG NHAU ĐỂ ĂN GIẢI";

            // Grid chứa 9 ô cào (3x3)
            GameObject gridObj = CreateUIElement("CellsGridContainer", ticketBoard.transform);
            RectTransform gRt = gridObj.GetComponent<RectTransform>();
            gRt.anchorMin = new Vector2(0.08f, 0.05f);
            gRt.anchorMax = new Vector2(0.92f, 0.78f);
            gRt.sizeDelta = Vector2.zero;
            GridLayoutGroup grid = gridObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(130f, 105f);
            grid.spacing = new Vector2(14f, 10f);
            grid.childAlignment = TextAnchor.MiddleCenter;

            // --- RESULT BANNER (Banner chúc mừng) ---
            GameObject resultBanner = CreateUIElement("ResultBanner", ticketBoard.transform);
            RectTransform rbRt = resultBanner.GetComponent<RectTransform>();
            rbRt.anchorMin = new Vector2(0.05f, 0.20f);
            rbRt.anchorMax = new Vector2(0.95f, 0.65f);
            rbRt.sizeDelta = Vector2.zero;
            Image rbImg = resultBanner.AddComponent<Image>();
            rbImg.color = new Color(0.08f, 0.25f, 0.15f, 0.95f);
            resultBanner.SetActive(false);

            GameObject resTitleObj = CreateUIElement("TxtResultTitle", resultBanner.transform);
            RectTransform rtRt = resTitleObj.GetComponent<RectTransform>();
            rtRt.anchorMin = new Vector2(0.05f, 0.55f);
            rtRt.anchorMax = new Vector2(0.95f, 0.95f);
            rtRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtResTitle = resTitleObj.AddComponent<TextMeshProUGUI>();
            txtResTitle.font = font;
            txtResTitle.fontSize = 24f;
            txtResTitle.fontStyle = FontStyles.Bold;
            txtResTitle.color = new Color(1f, 0.85f, 0.2f);
            txtResTitle.alignment = TextAlignmentOptions.Center;
            txtResTitle.text = "🎉 CHÚC MỪNG BẠN ĐÃ TRÚNG THƯỞNG! 🎉";

            GameObject resRewardObj = CreateUIElement("TxtResultReward", resultBanner.transform);
            RectTransform rrRt = resRewardObj.GetComponent<RectTransform>();
            rrRt.anchorMin = new Vector2(0.05f, 0.1f);
            rrRt.anchorMax = new Vector2(0.95f, 0.5f);
            rrRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI txtResReward = resRewardObj.AddComponent<TextMeshProUGUI>();
            txtResReward.font = font;
            txtResReward.fontSize = 32f;
            txtResReward.fontStyle = FontStyles.Bold;
            txtResReward.color = new Color(0.2f, 1f, 0.4f);
            txtResReward.alignment = TextAlignmentOptions.Center;
            txtResReward.text = "+$500";

            // --- BOTTOM CONTROLS (Thanh nút bấm dưới cùng) ---
            GameObject bottomBar = CreateUIElement("BottomBar", window.transform);
            RectTransform bRt = bottomBar.GetComponent<RectTransform>();
            bRt.anchorMin = new Vector2(0.04f, 0.02f);
            bRt.anchorMax = new Vector2(0.96f, 0.10f);
            bRt.sizeDelta = Vector2.zero;

            GameObject buyBtnObj = CreateButton("BtnBuyTicket", bottomBar.transform, new Vector2(0.40f, 0f), new Vector2(0.70f, 1f), "MUA VÉ ($50)", new Color(0.85f, 0.65f, 0.1f), font, 18f);
            Button btnBuy = buyBtnObj.GetComponent<Button>();

            GameObject scratchAllBtnObj = CreateButton("BtnScratchAll", bottomBar.transform, new Vector2(0.72f, 0f), new Vector2(0.98f, 1f), "CÀO TẤT CẢ", new Color(0.2f, 0.5f, 0.8f), font, 16f);
            Button btnScratchAll = scratchAllBtnObj.GetComponent<Button>();

            GameObject claimBtnObj = CreateButton("BtnClaimReward", bottomBar.transform, new Vector2(0.40f, 0f), new Vector2(0.70f, 1f), "🎉 NHẬN THƯỞNG 🎉", new Color(0.15f, 0.75f, 0.35f), font, 18f);
            Button btnClaim = claimBtnObj.GetComponent<Button>();
            claimBtnObj.SetActive(false);

            GameObject playAgainBtnObj = CreateButton("BtnPlayAgain", bottomBar.transform, new Vector2(0.40f, 0f), new Vector2(0.70f, 1f), "CHƠI TIẾP", new Color(0.85f, 0.65f, 0.1f), font, 18f);
            Button btnPlayAgain = playAgainBtnObj.GetComponent<Button>();
            playAgainBtnObj.SetActive(false);

            // Gán component ScratchTicketMachineView
            ScratchTicketMachineView machineView = root.AddComponent<ScratchTicketMachineView>();
            SerializedObject so = new SerializedObject(machineView);
            so.FindProperty("txtPlayerMoney").objectReferenceValue = txtMoney;
            so.FindProperty("txtMachineTitle").objectReferenceValue = txtTitle;
            so.FindProperty("txtStatusMessage").objectReferenceValue = txtStatus;

            so.FindProperty("btnTierBronze").objectReferenceValue = btnBronze;
            so.FindProperty("btnTierSilver").objectReferenceValue = btnSilver;
            so.FindProperty("btnTierGold").objectReferenceValue = btnGold;
            so.FindProperty("imgTierBronzeBorder").objectReferenceValue = imgBronzeBorder;
            so.FindProperty("imgTierSilverBorder").objectReferenceValue = imgSilverBorder;
            so.FindProperty("imgTierGoldBorder").objectReferenceValue = imgGoldBorder;

            so.FindProperty("ticketContainer").objectReferenceValue = ticketBoard;
            so.FindProperty("txtTicketTitle").objectReferenceValue = txtTicketTitle;
            so.FindProperty("txtTicketRule").objectReferenceValue = txtTicketRule;
            so.FindProperty("cellsGridContainer").objectReferenceValue = gridObj.transform;
            so.FindProperty("cellViewPrefab").objectReferenceValue = cellPrefab;

            so.FindProperty("btnBuyTicket").objectReferenceValue = btnBuy;
            so.FindProperty("btnScratchAll").objectReferenceValue = btnScratchAll;
            so.FindProperty("btnClaimReward").objectReferenceValue = btnClaim;
            so.FindProperty("btnPlayAgain").objectReferenceValue = btnPlayAgain;
            so.FindProperty("btnClose").objectReferenceValue = btnClose;

            so.FindProperty("resultBanner").objectReferenceValue = resultBanner;
            so.FindProperty("txtResultTitle").objectReferenceValue = txtResTitle;
            so.FindProperty("txtResultReward").objectReferenceValue = txtResReward;

            so.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject CreateButton(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, string label, Color color, TMP_FontAsset font, float fontSize)
        {
            GameObject btnObj = CreateUIElement(name, parent);
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.sizeDelta = Vector2.zero;

            Image img = btnObj.AddComponent<Image>();
            img.color = color;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.highlightedColor = color * 1.15f;
            cb.pressedColor = color * 0.85f;
            cb.disabledColor = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 0.5f);
            btn.colors = cb;

            GameObject textObj = CreateUIElement("TxtLabel", btnObj.transform);
            RectTransform tRt = textObj.GetComponent<RectTransform>();
            tRt.anchorMin = Vector2.zero;
            tRt.anchorMax = Vector2.one;
            tRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = label;

            return btnObj;
        }

        private static void RegisterViewInAddressables(string assetPath)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) return;

            AddressableAssetGroup group = settings.DefaultGroup;
            if (group == null) return;

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
            if (entry != null)
            {
                entry.address = assetPath;
                entry.SetLabel("view", true, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            }
        }

        private static void ConnectViewToMachine(ScratchTicketMachineView viewPrefab)
        {
            if (viewPrefab == null) return;

            // 1. Gán vào Prefab "Lottery Machine.prefab"
            string machinePrefabPath = "Assets/Prefabs/Lottery Machine.prefab";
            if (File.Exists(machinePrefabPath))
            {
                GameObject machinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(machinePrefabPath);
                if (machinePrefab != null)
                {
                    ScratchTicketMachine stm = machinePrefab.GetComponent<ScratchTicketMachine>();
                    if (stm != null)
                    {
                        SerializedObject so = new SerializedObject(stm);
                        SerializedProperty prop = so.FindProperty("fallbackViewPrefab");
                        if (prop != null)
                        {
                            prop.objectReferenceValue = viewPrefab;
                            so.ApplyModifiedProperties();
                            EditorUtility.SetDirty(machinePrefab);
                        }
                    }
                }
            }

            // 2. Gán vào GameObject "Lottery Machine" trong active scene
            GameObject sceneMachine = GameObject.Find("Lottery Machine");
            if (sceneMachine != null)
            {
                ScratchTicketMachine stm = sceneMachine.GetComponent<ScratchTicketMachine>();
                if (stm != null)
                {
                    SerializedObject so = new SerializedObject(stm);
                    SerializedProperty prop = so.FindProperty("fallbackViewPrefab");
                    if (prop != null)
                    {
                        prop.objectReferenceValue = viewPrefab;
                        so.ApplyModifiedProperties();
                        EditorSceneManager.MarkSceneDirty(sceneMachine.scene);
                        EditorSceneManager.SaveScene(sceneMachine.scene);
                    }
                }
            }
        }
    }
}

