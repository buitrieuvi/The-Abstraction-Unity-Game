using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways] // Cho phép chạy thử trực tiếp trên Editor khi thay đổi thông số (tuỳ chọn)
public class PlanetRingManager : MonoBehaviour
{
    [Header("Cài đặt Prefab Cube")]
    public GameObject cubePrefab;       // Prefab của Cube cần rải
    public int totalCubes = 100;        // Mật độ: Tổng số lượng cube trong vành đai

    [Header("Hình dáng & Kích thước Vành đai")]
    public float innerRadius = 5f;      // Bán kính trong của vành đai
    public float outerRadius = 9f;      // Bán kính ngoài của vành đai
    [Range(0.1f, 2f)]
    public float ellipseRatio = 1f;     // Tỉ lệ hình dáng (1f = hình tròn hoàn hảo, khác 1f = hình elip)

    [Header("Tốc độ & Chuyển động")]
    public float baseRotationSpeed = 20f; // Tốc độ xoay chung của cả vành đai (độ/giây)
    public bool randomizeCubeSpeed = true; // Các cube có tốc độ tự xoay khác nhau không

    [Header("Độ nhiễu (Tạo sự tự nhiên)")]
    public float heightVariation = 0.5f; // Độ nhô lên/xuống theo trục Y để vành trông dày dặn

    private List<Transform> spawnedCubes = new List<Transform>();

    void Start()
    {
        // Chỉ tạo cube khi chạy game (hoặc bạn có thể bấm nút tạo trong Editor)
        if (Application.isPlaying)
        {
            GenerateRing();
        }
    }

    [ContextMenu("Generate Ring Now")]
    public void GenerateRing()
    {
        // Xóa các cube cũ nếu đã tạo trước đó
        foreach (Transform child in spawnedCubes)
        {
            if (child != null)
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }
        }
        spawnedCubes.Clear();

        if (cubePrefab == null)
        {
            Debug.LogWarning("Vui lòng gán Prefab của Cube vào Script!");
            return;
        }

        // Tạo vòng lặp rải cube theo mật độ
        for (int i = 0; i < totalCubes; i++)
        {
            // Random khoảng cách từ bán kính trong đến bán kính ngoài
            float radius = Random.Range(innerRadius, outerRadius);

            // Phân bổ góc đều theo vòng tròn (360 độ) kết hợp chút ngẫu nhiên để không bị đơ
            float angle = i * (361f / totalCubes) * Mathf.Deg2Rad;

            // Tính tọa độ X, Z (hỗ trợ hình elip thông qua ellipseRatio)
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius * ellipseRatio;
            float y = Random.Range(-heightVariation, heightVariation);

            Vector3 spawnPos = transform.position + new Vector3(x, y, z);

            // Spawn Cube và gom nhóm làm con của GameObject này
            GameObject newCube = Instantiate(cubePrefab, spawnPos, Random.rotation, transform);

            // Gắn component phụ trách tự xoay cho từng cube nếu muốn
            RingCubeItem item = newCube.GetComponent<RingCubeItem>();
            if (item == null) item = newCube.AddComponent<RingCubeItem>();

            item.orbitAngle = angle * Mathf.Rad2Deg;
            item.radius = radius;
            item.ellipseRatio = ellipseRatio;
            item.rotationSpeed = randomizeCubeSpeed ? baseRotationSpeed * Random.Range(0.8f, 1.2f) : baseRotationSpeed;

            spawnedCubes.Add(newCube.transform);
        }
    }

    void Update()
    {
        // Cho cả vành đai xoay tròn quanh tâm (Trục Y)
        transform.Rotate(Vector3.up, baseRotationSpeed * Time.deltaTime);
    }
}
