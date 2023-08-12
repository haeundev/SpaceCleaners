using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    private int initialTaskID = 1;
    public static TaskManager Instance;
    public TaskInfos taskInfos;
    private List<TaskInfo> _tasks;
    public TaskInfo CurrentTask { get; private set; }
    public event Action<TaskInfo> OnTaskAcquired;
    public event Action<TaskInfo> OnDialogueTaskInit;

    private void Awake()
    {
        Instance = this;
        taskInfos = DataTableManager.TaskInfos;
        _tasks = taskInfos.Values;
    }

    private void Start()
    {
        CurrentTask = _tasks.First(p => p.ID == initialTaskID);
        OnTaskAcquired += InitTask;
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "OuterSpace")
            {
                CompleteCurrentTask();
            }
        };
    }

    private void InitTask(TaskInfo taskInfo)
    {
        if (taskInfo == default)
            return;
        
        switch (taskInfo.TaskType)
        {
            case TaskType.None:
                break;
            case TaskType.Dialogue:
                OnDialogueTaskInit?.Invoke(taskInfo);
                break;
            case TaskType.Instruction:
                break;
        }
    }

    // private void DisplayTitle(TaskInfo taskInfo)
    // {
    //     SoundSources.StopAll(false);
    //     StartCoroutine(coDisplayTitle(taskInfo));
    // }
    //
    // private IEnumerator coDisplayTitle(TaskInfo taskInfo)
    // {
    //     yield return new WaitUntil(() => CloseScreen.Instance.IsClosing == false);
    //     var win = UIWindows.GetWindow(1) as UI_Toast_Title;
    //     win.SetTitle(taskInfo.ValueStr);
    //     win.Open();
    //     yield return new WaitForSeconds(3f);
    //     Debug.Log("Title display done.");
    //     CompleteCurrentTask();
    // }
    //
    // // private IEnumerator WaitForSec(float sec, Action onEnd)
    // // {
    // //     yield return new WaitForSeconds(sec);
    // //     onEnd?.Invoke();
    // // }
    //
    // public void ResetToFirstTaskOfGroup()
    // {
    //     UIWindows.CloseAll();
    //     SoundSources.StopAll(true);
    //     
    //     var first = _tasks.FirstOrDefault(p => p.Group == CurrentTask.Group);
    //     CurrentTask = first;
    //     OnTaskAcquired?.Invoke(CurrentTask);
    //
    //     StartCoroutine(SpawnPlayer(first.Group.ToString()));
    // }
    //
    // private void Update()
    // {
    //     var keyboardInput = UIWindows.GetWindow(6);
    //     if (keyboardInput.gameObject.activeSelf)
    //         return;
    //
    //     if (Input.GetKey(KeyCode.LeftShift))
    //     {
    //         if (Input.GetKeyDown(KeyCode.R))
    //         {
    //             ResetToFirstTaskOfGroup();
    //         }
    //         else if (Input.GetKeyDown(KeyCode.H))
    //         {
    //             ShowHint();
    //         }
    //     }
    // }

    private void ShowHint()
    {
        
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
            Debug.Log($"[TaskManager]  OnTaskAcquired {CurrentTask.ID}");
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
        // switch (values[0])
        // {
        //     case "stop_sound":
        //         var taskToStopAudio = _tasks.Find(p => p.ID == int.Parse(values[1]));
        //         SoundSources.Stop(taskToStopAudio.ValueStr);
        //         break;
        //
        //     case "spawn":
        //         UIWindows.GetWindow(1).enabled = false;
        //         yield return SpawnPlayer(values[1]);
        //         break;
        // }
        
        yield break;
    }
}