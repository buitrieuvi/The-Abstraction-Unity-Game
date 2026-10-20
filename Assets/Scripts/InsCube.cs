using Cysharp.Threading.Tasks;
using UnityEngine;

public class InsCube : MonoBehaviour
{
    public GameObject Prefab;

    // Dùng Start thay vì Update
    async void Start()
    {
        while (true)
        {
            Instantiate(Prefab, transform);

            await UniTask.WaitForSeconds(0.34f);
        }
    }
}