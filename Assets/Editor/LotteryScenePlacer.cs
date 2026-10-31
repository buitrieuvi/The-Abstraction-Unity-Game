using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lottery.Editor
{
    [InitializeOnLoad]
    public static class LotteryScenePlacer
    {
        private const string MachineName = "Lottery Machine";
        private const string PrefabPath = "Assets/Prefabs/Lottery Machine.prefab";

        static LotteryScenePlacer()
        {
            EditorApplication.delayCall += OnEditorReady;
        }

        private static void OnEditorReady()
        {
            PlaceOrUpdateMachine(false);
        }

        [MenuItem("Lottery/Create & Place 3D Lottery Machine")]
        public static void ForcePlaceMachine()
        {
            PlaceOrUpdateMachine(true);
        }

        public static void PlaceOrUpdateMachine(bool forceRecreate)
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (!activeScene.isLoaded) return;

            GameObject existing = GameObject.Find(MachineName);
            if (existing != null)
            {
                if (!forceRecreate)
                {
                    // Đã tồn tại trong Scene, chỉ cần đảm bảo Visual đầy đủ
                    var existingVis = existing.GetComponent<LotteryMachineVisuals>();
                    if (existingVis != null)
                    {
                        existingVis.EnsureVisualHierarchy();
                    }
                    return;
                }
                else
                {
                    Object.DestroyImmediate(existing);
                }
            }

            // 1. Tạo GameObject gốc cho Máy Vé Số
            GameObject machine = new GameObject(MachineName);
            machine.transform.position = new Vector3(-3.5f, 1.0f, -25.0f);
            machine.transform.rotation = Quaternion.Euler(0, 90f, 0); // Xoay mặt trước về phía người chơi

            // 2. Thêm BoxCollider để bắt tương tác
            BoxCollider col = machine.AddComponent<BoxCollider>();
            col.size = new Vector3(1.8f, 2.8f, 1.6f);
            col.center = new Vector3(0, 1.4f, 0);

            // 3. Thêm ScratchTicketMachine (Logic Gameplay & Tương tác)
            ScratchTicketMachine ticketMachine = machine.AddComponent<ScratchTicketMachine>();

            // 4. Thêm LotteryMachineVisuals (Mô hình 3D Kiosk, hiệu ứng neon & animation)
            LotteryMachineVisuals visuals = machine.AddComponent<LotteryMachineVisuals>();
            visuals.EnsureVisualHierarchy();

            // 5. Thêm AdvancedOutline (Hiệu ứng viền sáng 3D)
            var outline = machine.AddComponent<ITISKIRUHERE.AdvancedOutline>();
            ticketMachine.outline3d = outline;

            // 6. Tạo thư mục Prefabs nếu chưa có và lưu Prefab
            string prefabDir = Path.GetDirectoryName(PrefabPath);
            if (!Directory.Exists(prefabDir))
            {
                Directory.CreateDirectory(prefabDir);
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(machine, PrefabPath, InteractionMode.AutomatedAction);

            // 7. Lưu Scene
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=#00FF88>[Lottery] Đã tạo thành công mô hình 3D Máy Bán Vé Số Cào và đặt vào Scene '{activeScene.name}' tại vị trí {machine.transform.position}!</color>");
        }
    }
}

