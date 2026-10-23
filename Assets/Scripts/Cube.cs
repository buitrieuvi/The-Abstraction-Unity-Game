using DG.Tweening;
using UnityEngine;

public class Cube : EventTrigger
{
    public Rigidbody Rb;
    private Renderer cubeRenderer; // Fixed: Used to change material colors properly
    private Tween tween;

    private void Start()
    {
        if (Rb == null)
            Rb = GetComponent<Rigidbody>();

        cubeRenderer = GetComponent<Renderer>();
    }

    public void PickupCube()
    {
        if (Rb == null || Camera.main == null)
            return;

        tween?.Kill();

        Rb.isKinematic = true;
        Rb.useGravity = false;
        Rb.linearVelocity = Vector3.zero;     // Use Rb.velocity if you are on Unity 2022 or older
        Rb.angularVelocity = Vector3.zero;

        transform.SetParent(Camera.main.transform, false);
        transform.localPosition = new Vector3(0f, 0f, 2f);
        transform.localRotation = Quaternion.identity;

        Ani_Cube(true);
    }

    public void ThrowCube()
    {
        if (Rb == null || Camera.main == null)
            return;

        Ani_Cube(false);

        transform.SetParent(null, true);

        Rb.isKinematic = false;
        Rb.useGravity = true;

        float throwForce = 15f;
        Rb.AddForce(
            Camera.main.transform.forward * throwForce,
            ForceMode.Impulse
        );
    }

    public void Ani_Cube(bool isPlay)
    {
        tween?.Kill();

        if (!isPlay)
            return;

        tween = transform
            .DOLocalRotate(
                new Vector3(0f, 360f, 0f),
                2f,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }

    private void OnDestroy()
    {
        tween?.Kill();
    }

    public void Rand()
    {
        float x = Random.Range(0.1f, .3f);
        float y = Random.Range(0.1f, .3f);
        float z = Random.Range(0.1f, .3f);

        transform.localScale = new Vector3(x, y, z);

        //if (cubeRenderer != null)
        //{
        //    cubeRenderer.material.color = Random.ColorHSV(1, 2, 3, 4);
        //}
    }

    public float GetValue() 
    {
        return (transform.localScale.x + transform.localScale.y + transform.localScale.z) /3;
    }

    public void OnIdle() 
    {
        Rb.isKinematic = transform;
        Rb.useGravity = false;
    }
}