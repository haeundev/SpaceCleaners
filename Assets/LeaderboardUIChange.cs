using System;
using System.Collections;
using System.Collections.Generic;
using DataTables;
using LiveLarson.Enums;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIChange : MonoBehaviour
{
    public GameObject leaderboardImg;

    public Sprite[] leaderboardImgs;

    private Sprite currentLeaderboardSprite;
    // Start is called before the first frame update
    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;

        currentLeaderboardSprite = leaderboardImg.GetComponent<Image>().sprite;
        currentLeaderboardSprite = leaderboardImgs[0];
    }

    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }

    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        currentLeaderboardSprite = taskInfo.ID switch
        {
            5 => leaderboardImgs[1],
            12 => leaderboardImgs[2],
            14 => leaderboardImgs[3],
            _ => currentLeaderboardSprite
        };
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}