using UnityEngine;

public class TaskCompleteUI : MonoBehaviour
{
    private void Awake()
    {
        TaskManager.Instance.taskCompleteUI = gameObject;
    }
}