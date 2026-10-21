
using DG.Tweening;
using UnityEngine;

public class Cube : EventTrigger
{
    public Rigidbody Rb;

    private Tween tween;

    private void Start()
    {
        if (Rb == null)
            Rb = GetComponent<Rigidbody>();
    }

    public void PickupCube()
    {
        if (Rb == null || Camera.main == null)
            return;

        tween?.Kill();

        Rb.isKinematic = true;
        Rb.useGravity = false;
        Rb.linearVelocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;

        transform.SetParent(Camera.main.transform, false);
        transform.localPosition = new Vector3(0f, 0f, 2f);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * 0.1f;

        Ani_Cube(true);
    }

    public void ThrowCube()
    {
        if (Rb == null || Camera.main == null)
            return;

        Ani_Cube(false);

        transform.SetParent(null, true);
        transform.localScale = Vector3.one * 0.5f;

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
}
