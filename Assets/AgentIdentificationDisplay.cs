using DevFeatures.SaveSystem;
using LiveLarson.DataTableManagement;
using TMPro;
using UnityEngine;

public class AgentIdentificationDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro agentName;
    [SerializeField] private TextMeshPro agentLevel;
    
    private void OnEnable()
    {
        agentName.text = SaveAndLoadManager.Instance.PlayerStat.name;

        var levelNames = DataTableManager.GameConst.Data.AgentLevelNames;
        var currentLevel = SaveAndLoadManager.Instance.PlayerStat.level; // 1 based index
        if (currentLevel < 1 || currentLevel > levelNames.Count)
        {
            Debug.LogError($"Something wrong with current level ({currentLevel})");
            return;
        }
        
        var currentLevelName = levelNames[currentLevel - 1];
        agentLevel.text = currentLevelName;
    }
}