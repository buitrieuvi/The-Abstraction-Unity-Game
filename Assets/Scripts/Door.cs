using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3.Triggers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Door : EventTrigger
{
    [SerializeField] GameObject l;
    [SerializeField] GameObject r;

    [SerializeField] float o;
    [SerializeField] float c;


    private void Start()
    {

    }

    public void OnOpen(bool b) 
    {
        l.transform?.DOKill();
        r.transform?.DOKill();

        if (b)
        {
            l.transform.DOMoveX(o, 2);
            r.transform.DOMoveX(-o, 2);
        }

        else 
        {
            l.transform.DOMoveX(c, 2);
            r.transform.DOMoveX(-c, 2);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        OnOpen(true);
    }

    public override async void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        await UniTask.WaitForSeconds(1.5f);

        OnOpen(false);
    }
}
