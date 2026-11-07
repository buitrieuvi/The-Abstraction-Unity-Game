using DG.Tweening;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Cube : EventTrigger
{
    public Rigidbody Rb;
    private Tween tween;

    private void Start()
    {
        if (Rb == null)
            Rb = GetComponent<Rigidbody>();


        transform.SetParent(null);

        float x = Mathf.Round(Mathf.Round(Random.Range(0.05f, 0.2f) * 100f)) / 100f;
        float y = Mathf.Round(Mathf.Round(Random.Range(0.05f, 0.2f) * 100f)) / 100f;
        float z = Mathf.Round(Mathf.Round(Random.Range(0.05f, 0.2f) * 100f)) / 100f;

        transform.localScale = new Vector3(x, y, z);

        //MachineSpawnCube machineSpawnCube = FindFirstObjectByType<MachineSpawnCube>();
        //if (machineSpawnCube != null)
        //{
        //    transform.SetParent(machineSpawnCube.farm);
        //}
    }

    public override void OnInteracted()
    {
        base.OnInteracted();
    }

    public override void OnPicked()
    {
        if (Rb == null || Camera.main == null)
            return;

        tween?.Kill();

        Rb.isKinematic = true;
        Rb.useGravity = false;
        Rb.linearVelocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;

        transform.SetParent(Camera.main.transform, false);
        transform.localPosition = new Vector3(0f, 0f, 1f);
        transform.localRotation = Quaternion.identity;

        Ani_Cube(true);
    }

    public override void OnThrowed()
    {

        if (Rb == null || Camera.main == null)
            return;

        Ani_Cube(false);

        transform.SetParent(null, true);

        Rb.isKinematic = false;
        Rb.useGravity = true;

        float throwForce = 20f;
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
    public float GetValue()
    {
        float value = transform.localScale.x + transform.localScale.y + transform.localScale.z;
        value = Mathf.Round(value * 100f) / 100f;
        return value;
    }
    public void OnIdle()
    {
        Rb.isKinematic = transform;
        Rb.useGravity = false;
    }

    public override void OnTriggerHoverEnter()
    {
        base.OnTriggerHoverEnter();

    }

    public override void OnTriggerHoverExit()
    {
        base.OnTriggerHoverExit();


    }
}