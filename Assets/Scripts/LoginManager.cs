using System;
using DevFeatures.SaveSystem;
using Unity.XR.Oculus.Input;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private void Awake()
    {
        ES3.Save("deviceUID", SystemInfo.deviceUniqueIdentifier);
    }

    private void Start()
    {
        CreatePlayerIfNone();

        var duid = ES3.Load<string>("deviceUID");
        Debug.LogError("====== DUID: " + duid);
        //
    }

    private void CreatePlayerIfNone()
    {
        // if (SaveAndLoadManager.IsPlayerCreated())
        // {
        //     SaveAndLoadManager.LoadAll();
        // }
        // else
        // {
        //     // var gameConst = DataTableManager.GameConst.Data;
        //     // var agentName =
        //     //     $"{gameConst.AgentNames.PeekRandom()} {gameConst.AgentNumbers.PeekRandom()} {gameConst.AgentGenerations.PeekRandom()}";
        //     // var agentLevel = 1;
        //     SaveAndLoadManager.CreateNewPlayer("Echo", 1);
        //     Debug.Log($"[LoginManager] Player Created with AgentName:{"Echo"}");
        // }
    }
}