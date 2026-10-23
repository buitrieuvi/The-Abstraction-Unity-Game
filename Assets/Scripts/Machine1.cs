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
            Destroy(c.gameObject);
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
