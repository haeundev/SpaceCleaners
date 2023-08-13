using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using DevFeatures.Dialogue;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using LiveLarson.Util;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public GameObject taskCompleteUI;
    public GameObject levelUpUI;
    public GameObject instructionUI;
    public GameObject dialogueUI;

    private const int InitialTaskID = 1;
    public static TaskManager Instance;
    public TaskInfos taskInfos;
    private List<TaskInfo> _tasks;
    private int _debrisCount;
    private bool _isCompleteConditionSatisfied;

    public TaskInfo CurrentTask { get; private set; }
    public event Action<TaskInfo> OnTaskAcquired;
    public event Action<TaskInfo> OnDialogueTaskInit;

    private void Awake()
    {
        Instance = this;
        taskInfos = DataTableManager.TaskInfos;
        _tasks = taskInfos.Values;
        
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
    }

    private void OnDebrisCaptured(GameObject _)
    {
        _debrisCount++;
        Debug.Log($"[TaskManager]  _debrisCount {_debrisCount}");
    }

    private void Start()
    {
        OnTaskAcquired += InitTask;
        CurrentTask = _tasks.First(p => p.ID == InitialTaskID); // called after data table manager
        OnTaskAcquired?.Invoke(CurrentTask);
    }

    private void InitTask(TaskInfo taskInfo)
    {
        if (taskInfo == default)
            return;

        Debug.Log($"[TaskManager]  InitTask {taskInfo.ID}");

        StartCoroutine(CheckTaskCompleteCondition());
        
        switch (taskInfo.TaskType)
        {
            case TaskType.Dialogue:
                OnDialogueTaskInit?.Invoke(taskInfo);
                break;
            case TaskType.Instruction:
                dialogueUI.SetActive(false);
                ShowInstruction(taskInfo);
                break;
        }
    }
    
    private void ShowInstruction(TaskInfo taskInfo)
    {
        instructionUI.GetComponent<InstructionUI>().SetText(taskInfo.Title, taskInfo.ValueStr);
        instructionUI.SetActive(true);
    }
    
    public IEnumerator CheckTaskCompleteCondition()
    {
        _isCompleteConditionSatisfied = false;
        var values = CurrentTask.CompleteCondition.Trim('(', ')', ' ').Replace(" ", string.Empty).Split(',');
        Debug.Log($"[TaskManager] condition type: {values[0]}");
        if (values.Length == 0)
        {
            _isCompleteConditionSatisfied = true;
            yield break;
        }
        
        switch (values[0])
        {
            case "spaceloaded":
                yield return YieldInstructionCache.WaitUntil(() => DialogueManager.Instance != default && taskCompleteUI != default);
                Debug.Log($"[TaskManager] spaceloaded task complete condition satisfied");
                break;
            case "level":
                yield return ShowLevelUp();
                Debug.Log($"[TaskManager] level task complete condition satisfied");
                break;
            case "debris":
                yield return CheckDebrisCount(int.Parse(values[1]));
                Debug.Log($"[TaskManager] debris task complete condition satisfied");
                break;
            case "dialoguefinish":
                yield return YieldInstructionCache.WaitUntil(() => DialogueManager.Instance.dialogueFinished);
                Debug.Log($"[TaskManager] dialoguefinish task complete condition satisfied");
                break;
            case "jungleloaded":
                break;
            case "junglecomplete":
                break;
            case "monumentloaded":
                break;
            case "monumentcomplete":
                break;
        }
        
        _isCompleteConditionSatisfied = true;
        DialogueManager.Instance.dialogueFinished = false;
        CompleteCurrentTask();
        Debug.Log($"[TaskManager] condition satisfied. complete task: {CurrentTask.ID}");
    }
    
    private IEnumerator ShowLevelUp()
    {
        levelUpUI.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(3f);
    }
    
    private IEnumerator CheckDebrisCount(int count)
    {
        _debrisCount = 0;
        yield return YieldInstructionCache.WaitUntil(() => _debrisCount == count);
    }
    
    public void CompleteCurrentTask()
    {
        StartCoroutine(RunEndActions(() =>
        {
            if (_tasks == default || CurrentTask == default)
                return;
            CurrentTask = _tasks.FirstOrDefault(p => p.ID == CurrentTask.ID + 1);
            if (CurrentTask == default)
                return;
            OnTaskAcquired?.Invoke(CurrentTask);
            Debug.Log($"[TaskManager]  acquired {CurrentTask.ID}");
        }));
    }

    public IEnumerator RunEndActions(Action onDone)
    {
        foreach (var endAction in CurrentTask.EndAction.SeparateAllParenthesis())
        {
            var values = endAction.Trim('(', ')', ' ').Replace(" ", string.Empty).Split(',');
            if (values.Length == 0)
                yield break;

            yield return DoEndAction(values);
        }
        
        // yield return new WaitUntil(() => CloseScreen.Instance.IsClosing == false);
        onDone?.Invoke();
    }

    private IEnumerator DoEndAction(IReadOnlyList<string> values)
    {
        switch (values[0])
        {
            case "taskend":
                yield return ShowTaskComplete();
                break;
            
            case "levelup":
                yield return ShowTaskComplete();
                break;
        }
        
        yield break;
    }

    private IEnumerator ShowTaskComplete()
    {
        taskCompleteUI.GetComponent<TaskCompleteUI>().SetText(CurrentTask.Title);
        taskCompleteUI.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(3f);
    }
}