using DataTables;
using UnityEngine;
using UnityEngine.UI;

public class ContestMapUIChange : MonoBehaviour
{
    public GameObject contestMapImg;
    public Sprite[] mapImgs;
    private Sprite _currentMapSprite;

    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;

        var initialSprite = contestMapImg.GetComponent<Image>().sprite = mapImgs[0];
        _currentMapSprite = initialSprite;
    }

    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }

    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        _currentMapSprite = taskInfo.ID switch
        {
            // 하드코딩
            4 => mapImgs[1],
            5 => mapImgs[2],
            8 => mapImgs[3],
            10 => mapImgs[4],
            13 => mapImgs[5],
            _ => _currentMapSprite
        };
    }
}