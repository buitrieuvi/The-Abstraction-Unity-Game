using UnityEngine;

public class RingCubeItem : MonoBehaviour
{
    [HideInInspector] public float orbitAngle;
    [HideInInspector] public float radius;
    [HideInInspector] public float ellipseRatio;
    [HideInInspector] public float rotationSpeed;

    void Update()
    {
        // Tự xoay quanh trục riêng của cube nhìn cho sinh động
        transform.Rotate(Vector3.one * (rotationSpeed * 2f * Time.deltaTime));
    }
}