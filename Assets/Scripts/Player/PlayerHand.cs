using DG.Tweening;
using Unity.Pipeline;
using UnityEngine;
using Zenject;

public class PlayerHand : MonoBehaviour
{
    public EventTrigger Handle;
    public EventTrigger HandleHover;

    [Inject] private InputManager input;

    private void Update()
    {
        Vector3 screenCenterPoint = new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            0f
        );

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            EventTrigger eventTrigger = hit.transform.GetComponent<EventTrigger>();

            if (HandleHover == eventTrigger)
            {
                return;
            }

            HandleHover?.TriggerHoverExit();

            HandleHover = eventTrigger;

            HandleHover?.TriggerHoverEnter();
        }
        else
        {
            HandleHover?.TriggerHoverExit();
            HandleHover = null;
        }
    }

    public void Start()
    {
        input.InputActions.Player.Interact.started += (e) =>
        {
            //Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            //RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    Transform objectHit = hit.transform;
            //    EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();

            //    eventTrigger?.OnInteracted();
            //}

            HandleHover?.OnInteracted();
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

        //input.InputActions.Player.Attack1.performed += (e) =>
        //{

        //    Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        //    Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        Transform objectHit = hit.transform;
        //        EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();

        //        eventTrigger?.OnInteracted();
        //    }

        //    //if (Handle != null)
        //    //{
        //    //    Cube c = Handle.GetComponent<Cube>();

        //    //    if (c != null) 
        //    //    {
        //    //        Debug.Log(c.transform.localScale);
        //    //    }

        //    //    //Handle.OnThrowed();
        //    //    //Handle = null;
        //    //}

        //    //Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        //    //Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        //    //RaycastHit hit;

        //    //if (Physics.Raycast(ray, out hit))
        //    //{
        //    //    Transform objectHit = hit.transform;
        //    //    EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();
        //    //    if (eventTrigger?.tag == "cube")
        //    //    {
        //    //        Handle = eventTrigger;
        //    //    }
        //    //    eventTrigger?.OnPicked();
        //    //}
        //};
    }

}
