using LiveLarson.SoundSystem;
using TMPro;
using UnityEngine;

public class TaskCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpTaskTitle;
    [SerializeField] private string sfx = "Assets/Audio/Capture Success.ogg";

    private void Awake()
    {
        TaskManager.Instance.taskCompleteUI = gameObject;
    }

    private void OnEnable()
    {
        SoundService.PlaySfx(sfx, transform.position);
    }

    public void SetText(string taskTitle)
    {
        tmpTaskTitle.text = $"미션 완료:\n{taskTitle}";
    }
}