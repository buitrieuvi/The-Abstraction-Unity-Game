using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Lottery
{
    /// <summary>
    /// Đối tượng Máy Bán Vé Số Cào 3D đặt trong thế giới game.
    /// Kế thừa từ EventTrigger theo đúng kiến trúc tương tác của dự án.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ScratchTicketMachine : EventTrigger
    {
        [Header("Cài đặt Máy Bán Vé Số")]
        [SerializeField] private string machineName = "Máy Bán Vé Số Cào May Mắn";
        public string MachineName => machineName;
        [SerializeField] private ScratchTicketMachineView fallbackViewPrefab;

        [Header("Hiệu ứng & Âm thanh")]
        [SerializeField] private LotteryMachineVisuals visuals;
        [SerializeField] private ParticleSystem interactParticle;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip interactSound;
        [SerializeField] private AudioClip buySound;
        [SerializeField] private AudioClip winSound;

        [Header("Hoạt cảnh lồng quay / màn hình máy (Tùy chọn)")]
        [SerializeField] private Transform spinDisplay;
        [SerializeField] private float spinSpeed = 45f;

        private ScratchTicketController controller;
        private ScratchTicketModel model;
        private ScratchTicketMachineView activeInstanceView;

        private void Start()
        {
            model = new ScratchTicketModel();
            if (visuals == null) visuals = GetComponent<LotteryMachineVisuals>();
        }

        private void Update()
        {
            // Xoay nhẹ vật phẩm hoặc biểu tượng trên đầu máy để thu hút người chơi
            if (spinDisplay != null)
            {
                spinDisplay.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
            }
        }

        public override async void OnInteracted()
        {
            base.OnInteracted();

            if (controller == null)
            {
                controller = new ScratchTicketController(dataManager, model);

                controller.OnTicketPurchased += (ticket) =>
                {
                    if (audioSource != null && buySound != null)
                        audioSource.PlayOneShot(buySound);
                };

                controller.OnRewardClaimed += (amount, itemId) =>
                {
                    if (audioSource != null && winSound != null)
                        audioSource.PlayOneShot(winSound);

                    if (interactParticle != null)
                        interactParticle.Play();

                    if (visuals != null)
                        visuals.PlayWinCelebration();
                };
            }

            // Hoạt cảnh nảy máy bằng DOTween
            if (visuals != null)
            {
                visuals.PlayInteractBounce();
            }
            else
            {
                transform.DOKill();
                transform.DOPunchScale(Vector3.up * 0.12f, 0.35f, 4, 0.4f);
            }

            // Âm thanh tương tác
            if (audioSource != null && interactSound != null)
            {
                audioSource.PlayOneShot(interactSound);
            }

            if (interactParticle != null)
            {
                interactParticle.Play();
            }

            // Mở UI View theo kiến trúc ViewManager
            if (viewManager != null)
            {
                // Thử tải qua ViewManager (nếu viewPrefab đã đăng ký Addressable label 'view')
                var view = await viewManager.ShowView<ScratchTicketMachineView>(controller);

                // Nếu chưa gán vào Addressables nhưng có kéo fallbackViewPrefab trong Inspector
                if (view == null && fallbackViewPrefab != null)
                {
                    OpenFallbackView();
                }
            }
            else if (fallbackViewPrefab != null)
            {
                OpenFallbackView();
            }
        }

        private void OpenFallbackView()
        {
            if (activeInstanceView != null)
            {
                activeInstanceView.Close();
                Destroy(activeInstanceView.gameObject);
                activeInstanceView = null;
                return;
            }

            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            activeInstanceView = Instantiate(fallbackViewPrefab, canvas.transform);
            activeInstanceView.Open(controller);
        }

        public override void TriggerHoverEnter()
        {
            base.TriggerHoverEnter();
            if (outline3d != null)
            {
                outline3d.OutlineWidth = 10f;
            }
        }

        public override void TriggerHoverExit()
        {
            base.TriggerHoverExit();
            if (outline3d != null)
            {
                outline3d.OutlineWidth = 0f;
            }
        }
    }
}
