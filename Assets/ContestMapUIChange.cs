using System;
using System.Collections;
using System.Collections.Generic;
using DataTables;
using LiveLarson.Enums;
using UnityEngine;
using UnityEngine.UI;

public class ContestMapUIChange : MonoBehaviour
{
    public GameObject contestMapImg;

    public Sprite[] mapImgs;

    private Sprite currentMapSprite;
    // Start is called before the first frame update
    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;

        currentMapSprite = contestMapImg.GetComponent<Image>().sprite;
        currentMapSprite = mapImgs[0];
    }

    private void OnDestroy()
    {
        TaskManager.Instance.OnTaskAcquired -= OnTaskAcquired;
    }

    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        currentMapSprite = taskInfo.ID switch
        {
            4 => mapImgs[1],
            5 => mapImgs[2],
            8 => mapImgs[3],
            10 => mapImgs[4],
            13 => mapImgs[5],
            _ => currentMapSprite
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
