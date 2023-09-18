using DataTables;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using LiveLarson.SoundSystem;
using TMPro;
using UnityEngine;

public class InstructionUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro description;
    
    private void Awake()
    {
        TaskManager.Instance.instructionUI = gameObject;
        TaskManager.Instance.OnInitTask += OnInitTask;
    }

    private void OnDestroy()
    {
        TaskManager.Instance.OnInitTask -= OnInitTask;
    }

    private void OnInitTask(TaskInfo taskInfo)
    {
        ShowInstruction(taskInfo);
    }
    
    private void ShowInstruction(TaskInfo taskInfo)
    {
        SetText(taskInfo.Title, taskInfo.ValueStr);
        gameObject.SetActive(true);

        if (string.IsNullOrEmpty(taskInfo.ValueStr) == false
            && taskInfo.TaskType != TaskType.MissionOnPlanet)
        {
            SoundService.PlaySfx("Assets/Audio/ringtone 1.mp3", transform.position);
        }
    }

    public void SetText(string titleText, string descText)
    {
        title.text = titleText;
        description.text = descText;
        if (transform == default)
        {
            Debug.LogError($"Should not be null");
            return;
        }
        if (transform == default)
            return;
        SoundService.PlaySfx("Assets/Audio/Message Appear.ogg", transform.position);
    }
}
