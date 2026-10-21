using DG.Tweening;
using UnityEngine;
using Zenject;

public class CubePickThrow : MonoBehaviour
{
    private Cube cube = null;

    [Inject] private InputManager input;

    public void Start()
    {
        input.InputActions.Player.Attack.performed += (e) =>
        {
            // THƯỜNG TRƯỜNG HỢP 1: ĐANG CẦM CUBE -> THÌ NÉM ĐI
            if (cube != null)
            {
                cube.ThrowCube();
                cube = null;

                return;
            }

            // TRƯỜNG HỢP 2: CHƯA CẦM GÌ -> THÌ BẮN RAYCAST ĐỂ NHẶT
            Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                Cube cb = objectHit.GetComponent<Cube>();

                if (cb != null)
                {
                    cube = cb;
                    cube.PickupCube();
                }
            }
        };
    }

}
