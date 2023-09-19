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

    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        if (taskInfo.ID == 4)
        {
            currentMapSprite = mapImgs[1];
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
