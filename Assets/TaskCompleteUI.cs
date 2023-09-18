using TMPro;
using UnityEngine;

public class TaskCompleteUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpTaskTitle;
    
    private void Awake()
    {
        TaskManager.Instance.taskCompleteUI = gameObject;
    }
    
    public void SetText(string taskTitle)
    {
        tmpTaskTitle.text = $"{taskTitle}";
    }
}