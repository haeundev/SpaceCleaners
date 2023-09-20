using DataTables;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIChange : MonoBehaviour
{
    [SerializeField] private Image image;
    public Sprite[] leaderboardSprites;

    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;
    }
    
    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        image.sprite = leaderboardSprites[taskInfo.LeaderboardIndex];
    }
    
    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }
}