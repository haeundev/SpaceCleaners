using DataTables;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIChange : MonoBehaviour
{
    public GameObject leaderboardImg;
    public Sprite[] leaderboardImgs;
    private Sprite _currentLeaderboardSprite;
    
    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;

        var initialSprite = leaderboardImg.GetComponent<Image>().sprite = leaderboardImgs[0];
        _currentLeaderboardSprite = initialSprite;
    }
    
    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }
    
    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        leaderboardImg.GetComponent<Image>().sprite = taskInfo.ID switch
        {
            // 하드코딩
            5 => leaderboardImgs[1],
            12 => leaderboardImgs[2],
            14 => leaderboardImgs[3],
            _ => _currentLeaderboardSprite
        };
    }
}