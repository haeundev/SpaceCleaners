using LiveLarson.SoundSystem;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private string sfx = "Assets/Audio/Capture Success.ogg";

    private void Awake()
    {
        TaskManager.Instance.levelUpUI = gameObject;
    }
    
    private void OnEnable()
    {
        SoundService.PlaySfx(sfx, transform.position);
    }
}