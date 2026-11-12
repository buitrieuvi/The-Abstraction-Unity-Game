using UnityEngine;

public class BulletPS : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private PlayerSkills playerSkills;

    public void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        playerSkills = FindObjectOfType<PlayerSkills>();

    }

    private void OnDisable()
    {
        playerSkills.ReturnToPool(gameObject);
    }

}
