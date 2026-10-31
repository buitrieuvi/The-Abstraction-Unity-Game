using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Lottery
{
    [ExecuteAlways]
    public class LotteryMachineVisuals : MonoBehaviour
    {
        [Header("Tùy chỉnh Màu sắc & Ánh sáng")]
        [SerializeField] private Color bodyColor = new Color(0.85f, 0.15f, 0.15f); // Đỏ may mắn
        [SerializeField] private Color goldTrimColor = new Color(1.0f, 0.84f, 0.0f); // Vàng kim loại
        [SerializeField] private Color neonGlowColor = new Color(0.1f, 0.9f, 1.0f); // Xanh ngọc neon
        [SerializeField] private Color ticketColor = new Color(1.0f, 0.95f, 0.6f); // Vàng nhạt vé cào

        [Header("Thành phần chuyển động")]
        [SerializeField] private Transform floatingIcon;
        [SerializeField] private Transform haloRing;
        [SerializeField] private Transform ticketSlot;
        [SerializeField] private Transform sampleTicket;
        [SerializeField] private Light machineLight;

        [Header("Tốc độ Animation")]
        [SerializeField] private float bobbingSpeed = 2.5f;
        [SerializeField] private float bobbingHeight = 0.12f;
        [SerializeField] private float spinSpeed = 50f;

        private float startIconY = 3.1f;
        private float startTicketZ = 0.6f;
        private Material matBody;
        private Material matGold;
        private Material matNeon;
        private Material matTicket;
        private Material matScreen;

        private void Awake()
        {
            EnsureMaterials();
            EnsureVisualHierarchy();

            if (floatingIcon != null)
            {
                startIconY = floatingIcon.localPosition.y;
            }
            if (sampleTicket != null)
            {
                startTicketZ = sampleTicket.localPosition.z;
            }
        }

        private void Update()
        {
            float t = Application.isPlaying ? Time.time : (float)Time.realtimeSinceStartup;

            // 1. Animation biểu tượng lơ lửng & xoay tròn trên đỉnh máy
            if (floatingIcon != null)
            {
                float newY = startIconY + Mathf.Sin(t * bobbingSpeed) * bobbingHeight;
                floatingIcon.localPosition = new Vector3(floatingIcon.localPosition.x, newY, floatingIcon.localPosition.z);
                floatingIcon.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            }

            // 2. Animation vành đai hào quang xoay nghiêng
            if (haloRing != null)
            {
                haloRing.Rotate(new Vector3(25f, 70f, 15f) * Time.deltaTime, Space.Self);
            }

            // 3. Animation vé cào mẫu nhấp nhô nhả vé
            if (sampleTicket != null)
            {
                float newZ = startTicketZ + Mathf.Sin(t * 3.5f) * 0.03f;
                sampleTicket.localPosition = new Vector3(sampleTicket.localPosition.x, sampleTicket.localPosition.y, newZ);
            }

            // 4. Ánh sáng neon thở nhẹ (breathe effect)
            if (machineLight != null)
            {
                machineLight.intensity = 2.0f + Mathf.Sin(t * 3.0f) * 0.5f;
            }
        }

        public void PlayInteractBounce()
        {
            if (!Application.isPlaying) return;

            transform.DOKill();
            transform.DOPunchScale(new Vector3(0.12f, 0.22f, 0.12f), 0.45f, 6, 0.5f);

            if (floatingIcon != null)
            {
                floatingIcon.DOPunchScale(Vector3.one * 0.35f, 0.5f, 5, 0.5f);
                floatingIcon.DORotate(new Vector3(0, 720, 0), 0.6f, RotateMode.WorldAxisAdd);
            }

            if (machineLight != null)
            {
                machineLight.DOColor(Color.white, 0.2f).OnComplete(() =>
                {
                    machineLight.DOColor(goldTrimColor, 0.4f);
                });
            }
        }

        public void PlayWinCelebration()
        {
            if (!Application.isPlaying) return;

            transform.DOPunchPosition(Vector3.up * 0.25f, 0.6f, 8);

            if (floatingIcon != null)
            {
                floatingIcon.DOScale(Vector3.one * 1.5f, 0.3f)
                    .SetLoops(4, LoopType.Yoyo);
            }

            if (machineLight != null)
            {
                machineLight.DOColor(goldTrimColor, 0.15f)
                    .SetLoops(6, LoopType.Yoyo);
            }
        }

        [ContextMenu("Rebuild 3D Model Visuals")]
        public void EnsureVisualHierarchy()
        {
            // Kiểm tra xem đã có model con chưa
            Transform rootVisual = transform.Find("VisualRoot");
            if (rootVisual != null && rootVisual.childCount >= 6)
            {
                // Đã có cấu trúc đầy đủ, chỉ cần gán lại tham chiếu nếu thiếu
                if (floatingIcon == null) floatingIcon = rootVisual.Find("FloatingDiamond");
                if (haloRing == null) haloRing = rootVisual.Find("HaloRing");
                if (sampleTicket == null) sampleTicket = rootVisual.Find("TicketSlot/SampleTicket");
                if (machineLight == null) machineLight = GetComponentInChildren<Light>();
                return;
            }

            if (rootVisual != null)
            {
                if (Application.isPlaying) Destroy(rootVisual.gameObject);
                else DestroyImmediate(rootVisual.gameObject);
            }

            EnsureMaterials();

            // Tạo gốc VisualRoot
            GameObject rootObj = new GameObject("VisualRoot");
            rootObj.transform.SetParent(transform, false);
            rootVisual = rootObj.transform;

            // 1. Đế máy (Base Plinth)
            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseObj.name = "BasePlinth";
            baseObj.transform.SetParent(rootVisual, false);
            baseObj.transform.localPosition = new Vector3(0, 0.15f, 0);
            baseObj.transform.localScale = new Vector3(1.7f, 0.3f, 1.5f);
            SetMaterial(baseObj, matGold);
            RemoveCollider(baseObj);

            // 2. Thân máy Kiosk (Main Cabinet)
            GameObject bodyObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bodyObj.name = "MainCabinet";
            bodyObj.transform.SetParent(rootVisual, false);
            bodyObj.transform.localPosition = new Vector3(0, 1.25f, 0);
            bodyObj.transform.localScale = new Vector3(1.4f, 1.9f, 1.2f);
            SetMaterial(bodyObj, matBody);
            RemoveCollider(bodyObj);

            // 3. Mái trên vát góc (Sloped Top Canopy)
            GameObject topObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topObj.name = "TopCanopy";
            topObj.transform.SetParent(rootVisual, false);
            topObj.transform.localPosition = new Vector3(0, 2.3f, 0.05f);
            topObj.transform.localRotation = Quaternion.Euler(15f, 0, 0);
            topObj.transform.localScale = new Vector3(1.5f, 0.45f, 1.3f);
            SetMaterial(topObj, matGold);
            RemoveCollider(topObj);

            // 4. Màn hình phát sáng Neon (Neon Screen)
            GameObject screenObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screenObj.name = "NeonScreen";
            screenObj.transform.SetParent(rootVisual, false);
            screenObj.transform.localPosition = new Vector3(0, 1.7f, 0.61f);
            screenObj.transform.localScale = new Vector3(1.05f, 0.75f, 0.05f);
            SetMaterial(screenObj, matScreen);
            RemoveCollider(screenObj);

            // 5. Khung viền màn hình (Screen Golden Bezel)
            GameObject bezelObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bezelObj.name = "ScreenBezel";
            bezelObj.transform.SetParent(rootVisual, false);
            bezelObj.transform.localPosition = new Vector3(0, 1.7f, 0.605f);
            bezelObj.transform.localScale = new Vector3(1.15f, 0.85f, 0.04f);
            SetMaterial(bezelObj, matGold);
            RemoveCollider(bezelObj);

            // 6. Khe nhả vé (Ticket Slot Box)
            GameObject slotObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slotObj.name = "TicketSlot";
            slotObj.transform.SetParent(rootVisual, false);
            slotObj.transform.localPosition = new Vector3(0, 0.9f, 0.62f);
            slotObj.transform.localScale = new Vector3(0.65f, 0.12f, 0.12f);
            SetMaterial(slotObj, matGold);
            RemoveCollider(slotObj);
            ticketSlot = slotObj.transform;

            // 7. Vé cào mẫu thò ra (Golden Sample Ticket)
            GameObject ticketObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ticketObj.name = "SampleTicket";
            ticketObj.transform.SetParent(ticketSlot, false);
            ticketObj.transform.localPosition = new Vector3(0, 0.05f, 0.6f);
            ticketObj.transform.localRotation = Quaternion.Euler(20f, 0, 0);
            ticketObj.transform.localScale = new Vector3(0.45f, 0.015f, 0.4f);
            SetMaterial(ticketObj, matTicket);
            RemoveCollider(ticketObj);
            sampleTicket = ticketObj.transform;

            // 8. Đỉnh gắn biểu tượng (Top Pedestal)
            GameObject pedestalObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestalObj.name = "TopPedestal";
            pedestalObj.transform.SetParent(rootVisual, false);
            pedestalObj.transform.localPosition = new Vector3(0, 2.6f, 0);
            pedestalObj.transform.localScale = new Vector3(0.55f, 0.1f, 0.55f);
            SetMaterial(pedestalObj, matGold);
            RemoveCollider(pedestalObj);

            // 9. Viên Kim Cương may mắn lơ lửng (Floating Diamond)
            GameObject diamondObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            diamondObj.name = "FloatingDiamond";
            diamondObj.transform.SetParent(rootVisual, false);
            diamondObj.transform.localPosition = new Vector3(0, startIconY, 0);
            diamondObj.transform.localRotation = Quaternion.Euler(45f, 45f, 45f); // Xoay chéo tạo hình khối kim cương
            diamondObj.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            SetMaterial(diamondObj, matNeon);
            RemoveCollider(diamondObj);
            floatingIcon = diamondObj.transform;

            // 10. Vành đai hào quang (Halo Ring)
            GameObject ringObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ringObj.name = "HaloRing";
            ringObj.transform.SetParent(floatingIcon, false);
            ringObj.transform.localPosition = Vector3.zero;
            ringObj.transform.localRotation = Quaternion.Euler(60f, 0, 30f);
            ringObj.transform.localScale = new Vector3(1.6f, 0.03f, 1.6f);
            SetMaterial(ringObj, matGold);
            RemoveCollider(ringObj);
            haloRing = ringObj.transform;

            // 11. Bảng hiệu 3D Text (World Space Marquee)
            Create3DMarqueeText(rootVisual);

            // 12. Ánh sáng Neon tỏa ra xung quanh (Point Light)
            GameObject lightObj = new GameObject("MachineNeonLight");
            lightObj.transform.SetParent(rootVisual, false);
            lightObj.transform.localPosition = new Vector3(0, 2.0f, 1.0f);
            machineLight = lightObj.AddComponent<Light>();
            machineLight.type = LightType.Point;
            machineLight.range = 5.0f;
            machineLight.intensity = 2.0f;
            machineLight.color = goldTrimColor;
        }

        private void Create3DMarqueeText(Transform parent)
        {
            GameObject marqueeObj = new GameObject("MarqueeText");
            marqueeObj.transform.SetParent(parent, false);
            marqueeObj.transform.localPosition = new Vector3(0, 2.75f, 0.45f);
            marqueeObj.transform.localRotation = Quaternion.Euler(15f, 0, 0);

            var tmp = marqueeObj.AddComponent<TextMeshPro>();
            tmp.text = "🎰 VÉ SỐ CÀO 🎰\n<size=70%><color=#FFD700>[F] MUA & CÀO</color></size>";
            tmp.fontSize = 3.6f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.rectTransform.sizeDelta = new Vector2(4f, 1.5f);
        }

        private void EnsureMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (matBody == null)
            {
                matBody = new Material(litShader) { name = "Mat_Lottery_Red" };
                matBody.SetColor("_BaseColor", bodyColor);
                matBody.SetFloat("_Smoothness", 0.65f);
            }

            if (matGold == null)
            {
                matGold = new Material(litShader) { name = "Mat_Lottery_Gold" };
                matGold.SetColor("_BaseColor", goldTrimColor);
                matGold.SetFloat("_Metallic", 0.85f);
                matGold.SetFloat("_Smoothness", 0.85f);
            }

            if (matNeon == null)
            {
                matNeon = new Material(litShader) { name = "Mat_Lottery_Neon" };
                matNeon.SetColor("_BaseColor", neonGlowColor);
                matNeon.EnableKeyword("_EMISSION");
                matNeon.SetColor("_EmissionColor", neonGlowColor * 1.5f);
                matNeon.SetFloat("_Smoothness", 0.9f);
            }

            if (matTicket == null)
            {
                matTicket = new Material(litShader) { name = "Mat_Lottery_Ticket" };
                matTicket.SetColor("_BaseColor", ticketColor);
                matTicket.SetFloat("_Smoothness", 0.7f);
            }

            if (matScreen == null)
            {
                matScreen = new Material(litShader) { name = "Mat_Lottery_Screen" };
                Color screenCol = new Color(0.05f, 0.35f, 0.5f);
                matScreen.SetColor("_BaseColor", screenCol);
                matScreen.EnableKeyword("_EMISSION");
                matScreen.SetColor("_EmissionColor", screenCol * 1.8f);
                matScreen.SetFloat("_Smoothness", 0.95f);
            }
        }

        private void SetMaterial(GameObject go, Material mat)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null && mat != null)
            {
                renderer.sharedMaterial = mat;
            }
        }

        private void RemoveCollider(GameObject go)
        {
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                if (Application.isPlaying) Destroy(col);
                else DestroyImmediate(col);
            }
        }
    }
}

