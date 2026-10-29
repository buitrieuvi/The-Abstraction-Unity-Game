using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildManager : MonoBehaviour
{
    public List<Child> Childrent = new();

    [Header("Lựa chọn trạng thái chung cho cả bầy")]
    public Child.PetState selectedState = Child.PetState.FollowPlayer;
    private Child.PetState lastState;

    public void Start()
    {
        // Tự động tìm tất cả các FollowPet trong các đối tượng con
        Childrent = transform.GetComponentsInChildren<Child>().ToList();
        lastState = selectedState;
        ApplyStateToAll(selectedState);
    }

    public void Update()
    {
        // Lọc bỏ các phần tử đã bị hủy
        Childrent.RemoveAll(child => child == null);

        // Nếu bạn thay đổi trạng thái trực tiếp trên Inspector khi đang chơi game
        if (selectedState != lastState)
        {
            ApplyStateToAll(selectedState);
            lastState = selectedState;
        }

        // Cập nhật hành vi di chuyển cho toàn bộ danh sách bầy đàn
        foreach (var item in Childrent)
        {
            if (item != null)
            {
                item.OnMove(Childrent);
            }
        }
    }

    // Hàm chuyển trạng thái toàn bầy từ các script khác hoặc UI
    public void ChangeAllPetsState(Child.PetState newState)
    {
        selectedState = newState;
        ApplyStateToAll(newState);
    }

    private void ApplyStateToAll(Child.PetState state)
    {
        foreach (var item in Childrent)
        {
            if (item != null)
            {
                item.SetState(state);
            }
        }
    }
}