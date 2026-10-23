using DG.Tweening;
using UnityEngine;

public class Machine1 : EventTrigger
{
    public override void OnOnInteracted()
    {
        base.OnOnInteracted();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Cube c = other.GetComponent<Cube>();

        if (c != null) 
        {
            float v = c.GetValue();

            c.OnIdle();
            c.Ani_ScaleZero();

            dataManager.Money += v;
            //Destroy(c.gameObject);
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Ani_UpDown();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
