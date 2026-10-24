using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class Machine1 : EventTrigger
{
    public override void OnOnInteracted()
    {
        base.OnOnInteracted();
    }

    bool isProsess;

    public override void OnTriggerEnter(Collider other)
    {
        if (isProsess) return;
        isProsess = true;

        base.OnTriggerEnter(other);

        Cube c = other.GetComponent<Cube>();

        if (c != null) 
        {
            float v = c.GetValue();

            c.OnIdle();

            dataManager.Money += v;
            dataManager.MoneyCube = v;
            Debug.Log(dataManager.MoneyCube);

            c.transform.DOScale(new Vector3(0, 0, 0), 0.95f).OnComplete(() =>
            {
                isProsess = false;
                Destroy(c);
            });

        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    void Start()
    {
        Ani_UpDown();
    }

}
