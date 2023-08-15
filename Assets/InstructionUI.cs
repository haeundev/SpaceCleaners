using DataTables;
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

    private void OnEnable()
    {
        ShowInstruction(TaskManager.Instance.CurrentTask);
    }

    private void OnInitTask(TaskInfo taskInfo)
    {
        ShowInstruction(taskInfo);
    }
    
    private void ShowInstruction(TaskInfo taskInfo)
    {
        SetText(taskInfo.Title, taskInfo.ValueStr);
        gameObject.SetActive(true);
    }

    public void SetText(string titleText, string descText)
    {
        title.text = titleText;
        description.text = descText;
        if (transform == default)
            return;
        SoundService.PlaySfx("Assets/Audio/Message Appear.ogg", transform.position);
    }
}
