using DevFeatures.SaveSystem;
using LiveLarson.DataTableManagement;
using LiveLarson.Util;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    private void Start()
    {
        CreatePlayerIfNone();
    }

    private void CreatePlayerIfNone()
    {
        if (SaveAndLoadManager.IsPlayerCreated())
        {
            SaveAndLoadManager.LoadAll();
        }
        else
        {
            var gameConst = DataTableManager.GameConst.Data;
            var agentName =
                $"{gameConst.AgentNames.PeekRandom()} {gameConst.AgentNumbers.PeekRandom()} {gameConst.AgentGenerations.PeekRandom()}";
            var agentLevel = 1;
            SaveAndLoadManager.CreateNewPlayer(agentName, agentLevel);
            Debug.Log($"[LoginManager] Player Created with AgentName:{agentName}");
        }
    }
}