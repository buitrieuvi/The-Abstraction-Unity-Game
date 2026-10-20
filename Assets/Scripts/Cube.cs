using UnityEngine;

public class Cube : EventTrigger
{
    public Rigidbody Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }
}
