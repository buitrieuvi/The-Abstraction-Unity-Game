using UnityEngine;

public class PlanetGravityPull : MonoBehaviour
{
    [Header("Cài đặt lực hút")]
    public float gravitationalPull = 10f; // Sức hút của hành tinh
    public float attractionRadius = 15f;  // Bán kính vùng hút

    void FixedUpdate()
    {
        // Tìm tất cả các đối tượng trong vùng ảnh hưởng có lớp (Layer) hoặc Tag là "Cube"
        Collider[] colliders = Physics.OverlapSphere(transform.position, attractionRadius);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Cube"))
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Tính hướng từ Cube về tâm hành tinh
                    Vector3 direction = transform.position - col.transform.position;
                    float distance = direction.magnitude;

                    // Càng lại gần lực hút càng mạnh (tùy chọn)
                    if (distance > 0.5f) // Tránh chia cho 0 hoặc dính chặt vào tâm
                    {
                        // Kéo cube về phía tâm hành tinh
                        Vector3 force = direction.normalized * (gravitationalPull / (distance * 0.5f));
                        rb.AddForce(force, ForceMode.Acceleration);
                    }
                }
            }
        }
    }

    // Vẽ bán kính vùng hút trên Unity Editor để dễ nhìn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}