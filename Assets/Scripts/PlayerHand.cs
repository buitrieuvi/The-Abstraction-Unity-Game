using DG.Tweening;
using Unity.Pipeline;
using UnityEngine;
using Zenject;

public class PlayerHand : MonoBehaviour
{
    public EventTrigger Handle;

    [Inject] private InputManager input;

    public void Start()
    {
        input.InputActions.Player.Interact.started += (e) =>
        {
            Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();

                eventTrigger?.OnInteracted();
            }
        };


        input.InputActions.Player.Attack.performed += (e) =>
        {
            if (Handle != null)
            {
                Handle.OnThrowed();
                Handle = null;
                return;
            }

            Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();
                if (eventTrigger?.tag == "cube")
                {
                    Handle = eventTrigger;
                }
                eventTrigger?.OnPicked();
            }
        };

        input.InputActions.Player.Attack1.performed += (e) =>
        {
            if (Handle != null)
            {
                Cube c = Handle.GetComponent<Cube>();

                if (c != null) 
                {
                    Debug.Log(c.transform.localScale);
                }

                //Handle.OnThrowed();
                //Handle = null;
            }

            //Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            //RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    Transform objectHit = hit.transform;
            //    EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();
            //    if (eventTrigger?.tag == "cube")
            //    {
            //        Handle = eventTrigger;
            //    }
            //    eventTrigger?.OnPicked();
            //}
        };
    }

}
