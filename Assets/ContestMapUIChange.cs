using DataTables;
using UnityEngine;
using UnityEngine.UI;

public class ContestMapUIChange : MonoBehaviour
{
    [SerializeField] private Image image;
    public Sprite[] mapSprites;

    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;
    }
    
    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        image.sprite = mapSprites[taskInfo.ContestMapIndex];
    }
    
    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }
}