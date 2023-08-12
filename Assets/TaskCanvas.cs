using DataTables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskCanvas : MonoBehaviour
{
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpDetail;
    
    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;
    }
    
    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        // characterIcon.sprite = GetSprite()
        tmpTitle.SetText(taskInfo.Title);
        tmpDetail.SetText(taskInfo.ValueStr);
    }
    
    // private void GetSprite()
}