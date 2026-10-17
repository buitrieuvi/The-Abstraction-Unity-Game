using DG.Tweening;
using R3.Triggers;
using TMPro;
using UnityEngine;

public class Door : EventTrigger
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] GameObject l;
    [SerializeField] GameObject r;

    [SerializeField] float o;
    [SerializeField] float c;


    [SerializeField] BoxCollider boxTrigger;

    private void Start()
    {

    }

    public void OnOpen(bool b) 
    {
        if (b)
        {
            l.transform.DOMoveX(o, 1);
            r.transform.DOMoveX(-o, 1);
        }

        else 
        {
            l.transform.DOMoveX(c, 1);
            r.transform.DOMoveX(-c, 1);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
