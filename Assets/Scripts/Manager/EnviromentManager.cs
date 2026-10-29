using DG.Tweening;
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{
    [SerializeField] private GameObject go1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUp() 
    {
        go1.transform.DOMoveY(10, 10f);
    }
}
