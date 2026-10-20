
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using Zenject;

public class Elevator : EventTrigger
{
    [Inject] public ViewManager ViewManager;

    public ElevatorController ElevatorController = new();

    [SerializeField] List<BoxCollider> doors;

    public bool isIn;


    public void Awake()
    {
        ElevatorController.Door.OnInteract += async () => 
        {
            ElevatorController.ElevatorView = (ElevatorView)await ViewManager.ShowView<ElevatorView>(ElevatorController);
        };

        //foreach (var door in doors)
        //{
        //    door.OnTriggerEnterAsObservable()
        //        .Where(e => e.CompareTag("Player"))
        //        .Subscribe(e =>
        //        {
        //            //Door(true, ElevatorController.numFloor);
        //        })
        //        .AddTo(this);
        //}

        //door.OnTriggerExitAsObservable()
        //    .Where(e => e.CompareTag("Player"))
        //    .Subscribe(async e =>
        //    {
        //        await UniTask.WaitForSeconds(5);
        //        Door(false);
        //    })
        //    .AddTo(this);

        //OnTriggerEnterEvent += () =>
        //{
        //    inputManager.InputActions.Player.Interact.started += HandleInteract;
        //};

        //OnTriggerExitEvent += () => 
        //{
        //    inputManager.InputActions.Player.Interact.started -= HandleInteract;
        //};
    }

    public void Start()
    {

    }


    public override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);
        //ElevatorController.Elevator = this;

        //other.transform.SetParent(transform);
        //isIn = true;
    }

    public override async void OnTriggerExit(Collider other)
    {
        //await UniTask.WaitForSeconds(2);
        //ElevatorController.GoFloor(0);

        //base.OnTriggerExit(other);
        //ElevatorController.Elevator = null;

        //other.transform.SetParent(null);
        //isIn = false;
    }

    //public async void HandleInteract(InputAction.CallbackContext _)
    //{
    //    ElevatorController.ElevatorView = (ElevatorView)await ViewManager.ShowView<ElevatorView>(ElevatorController);
    //}

    //public async void Door(bool b, int floor) 
    //{

    //    if (b) 
    //    {
    //        door_r.transform.DOLocalMoveX(-3.5f, 2);
    //        door_l.transform.DOLocalMoveX(3.5f, 2);

    //        ElevatorController.Door_floors[floor].GetChild(0).transform.DOLocalMoveX(8f, 1);
    //        ElevatorController.Door_floors[floor].GetChild(1).transform.DOLocalMoveX(-8f, 1);

    //        return;
    //    }

    //    door_r.transform.DOLocalMoveX(-1, 2);
    //    door_l.transform.DOLocalMoveX(1, 2);


    //    ElevatorController.Door_floors[floor].GetChild(0).transform.DOLocalMoveX(2.5f, 1);
    //    ElevatorController.Door_floors[floor].GetChild(1).transform.DOLocalMoveX(-2.5f, 1);

    //    ViewManager.OnlyClose();
    //}
}