using DG.Tweening;
using Unity.Pipeline;
using UnityEngine;
using Zenject;

public sealed class PlayerHand : MonoBehaviour
{
    //public EventTrigger Handle;
    //public EventTrigger HandleHover;

    //[Inject] public InputManager InputManager { get; private set; }

    //private void FixedUpdate()
    //{
    //    Vector3 screenCenterPoint = new Vector3(
    //        Screen.width / 2f,
    //        Screen.height / 2f,
    //        0f
    //    );

    //    Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        EventTrigger eventTrigger = hit.transform.GetComponent<EventTrigger>();

    //        if (HandleHover == eventTrigger)
    //        {
    //            return;
    //        }

    //        HandleHover?.OnTriggerHoverExit();

    //        HandleHover = eventTrigger;

    //        HandleHover?.OnTriggerHoverEnter();
    //    }
    //    else
    //    {
    //        HandleHover?.OnTriggerHoverExit();
    //        HandleHover = null;
    //    }
    //}

    //public void Start()
    //{

    //    InputManager.InputActions.Player.Interact.started += (e) =>
    //    {
    //        HandleHover?.OnInteracted();
    //    };


    //    InputManager.InputActions.Player.Attack.performed += (e) =>
    //    {
    //        if (Handle != null)
    //        {
    //            Handle.OnThrowed();
    //            Handle = null;
    //            return;
    //        }

    //        Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    //        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Transform objectHit = hit.transform;
    //            EventTrigger eventTrigger = objectHit.GetComponent<EventTrigger>();
    //            if (eventTrigger?.tag == "cube")
    //            {
    //                Handle = eventTrigger;
    //            }
    //            eventTrigger?.OnPicked();
    //        }
    //    };
    //}

}
