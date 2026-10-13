
using UnityEngine;

public class ElevatorController : ControllerBase
{
    public Elevator Elevator { get; set; }

    public void SelectFloor(int floorIndex)
    {
        if (Elevator == null)
        {
            Debug.LogWarning("Elevator chưa được khởi tạo.");
            return;
        }

        Elevator.GoFloor(floorIndex);
    }
}