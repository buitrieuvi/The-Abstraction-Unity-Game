using DG.Tweening;
using UnityEngine;

public class Enermy : EventTrigger
{
    public int HP = 100;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "cube")
        {
            HP -= 10;
        }

        gameObject.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.35f, 1);

        if (HP == 0) 
        {
            gameObject.SetActive(false);
        }

    }


}
