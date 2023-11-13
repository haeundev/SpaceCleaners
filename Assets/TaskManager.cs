using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using DevFeatures.Dialogue;
using LiveLarson.BootAndLoad;
using LiveLarson.Enums;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskManager : MonoBehaviour
{
    public GameObject taskCompleteUI;
    public GameObject levelUpUI;
    public GameObject instructionUI;
    public GameObject dialogueUI;
    
    private bool _isJungleDone;
    private bool _isJungleLoaded;
    private bool _isMonumentDone;
    private bool _isMonumentLoaded;
    
    public static TaskManager Instance;
    [SerializeField] private TaskInfos taskInfos;
    private List<TaskInfo> _tasks;
    private int _debrisCount;
    private bool _isCompleteConditionSatisfied;

    public event Action<TaskInfo> OnTaskAcquired;
    
    public TaskInfo CurrentTask { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        _tasks = taskInfos.Values;

        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
        JungleEvents.OnSceneComplete += () => _isJungleDone = true;
        JungleEvents.OnSceneLoaded += () => _isJungleLoaded = true;
        MonumentEvents.OnSceneComplete += () => _isMonumentDone = true;
        MonumentEvents.OnSceneLoaded += () => _isMonumentLoaded = true;

        // Application.quitting += OnAppQuit;
        
        GlobalValues.OnGlobalInitTaskSet += Init;
    }
    
    public void Init(int taskID)
    {
        OnTaskAcquired += InitTask;
        CurrentTask = taskInfos.Find(taskID); // called after data table manager
        InitTask(CurrentTask);
    }

    // private void OnAppQuit()
    // {
    //     SaveAndLoadManager.Instance.GameStat.currentTaskID = CurrentTask.ID;
    //     SaveAndLoadManager.Instance.Save(SaveAndLoadManager.Instance.GameStat);
    //     Debug.Log($"[TaskManager]  CurrentTask is saved as {CurrentTask.ID}");
    // }

    private void OnDebrisCaptured(DebrisType _, GameObject __)
    {
        _debrisCount++;
        Debug.Log($"[TaskManager]  _debrisCount {_debrisCount}");
    }

    private void Start()
    {
        // if (CurrentTask.ID == InitialTaskID)
        //     OnTaskAcquired?.Invoke(CurrentTask);
    }

    public event Action<TaskInfo> OnInitTask;

    private void InitTask(TaskInfo taskInfo)
    {
        if (taskInfo == default)
            return;

        if (taskInfo.TaskType == TaskType.EndGame)
        {
            Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
            {
                StopAllCoroutines();
                ApplicationContext.Instance.LoadScene("CreditScene");
            }).AddTo(this);
        }

        StartCoroutine(StartCheckReadyToInitTask(taskInfo));
    }

    private IEnumerator StartCheckReadyToInitTask(TaskInfo taskInfo)
    {
        yield return YieldInstructionCache.WaitUntil(() => DialogueManager.Instance != default);
        OnInitTask?.Invoke(taskInfo);
        Debug.Log($"[TaskManager]  InitTask {taskInfo.ID}");

        StartCoroutine(CheckTaskCompleteCondition());

        if (taskInfo.TaskType != TaskType.Dialogue)
            if (dialogueUI != default)
            {
                Debug.Log($"dialogueUI is null. taskInfo: {taskInfo.ID}");
                dialogueUI.SetActive(false);
            }

        StartCoroutine(DoStartActions(taskInfo.StartAction));
    }

    public IEnumerator CheckTaskCompleteCondition()
    {
        _isCompleteConditionSatisfied = false;
        var values = CurrentTask.CompleteCondition.Trim('(', ')', ' ').Replace(" ", string.Empty).Split(',');
        Debug.Log($"[TaskManager] condition type: {values[0]}");
        if (values.Length == 0)
        {
            Debug.LogError("[TaskManager] task must have completion condition");
            _isCompleteConditionSatisfied = true;
            yield break;
        }

        switch (values[0])
        {
            case "spaceloaded":
                yield return YieldInstructionCache.WaitUntil(() =>
                    DialogueManager.Instance != default && taskCompleteUI != default &&
                    SceneManager.GetActiveScene().name.Contains("Space"));
                Debug.Log("[TaskManager] spaceloaded task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "level":
                yield return ShowLevelUp();
                Debug.Log("[TaskManager] level task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "debris":
                yield return CheckDebrisCount(int.Parse(values[1]));
                Debug.Log("[TaskManager] debris task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "dialoguefinish":
                yield return YieldInstructionCache.WaitUntil(() => DialogueManager.Instance != default
                                                                   && DialogueManager.Instance.playedDialogueInScene
                                                                   && DialogueManager.Instance.dialogueFinished);
                Debug.Log("[TaskManager] dialoguefinish task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "jungleloaded":
                yield return YieldInstructionCache.WaitUntil(() => _isJungleLoaded);
                Debug.Log("[TaskManager] jungleloaded task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "junglecomplete":
                yield return YieldInstructionCache.WaitUntil(() => _isJungleDone);
                Debug.Log("[TaskManager] junglecomplete task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
            case "monumentloaded":
                yield return YieldInstructionCache.WaitUntil(() => _isMonumentLoaded);
                OnConditionSatisfied(CurrentTask);
                break;
            case "monumentcomplete":
                yield return YieldInstructionCache.WaitUntil(() => _isMonumentDone);
                Debug.Log("[TaskManager] junglecomplete task complete condition satisfied");
                OnConditionSatisfied(CurrentTask);
                break;
        }
    }

    private void OnConditionSatisfied(TaskInfo currentTask)
    {
        _isCompleteConditionSatisfied = true;
        DialogueManager.Instance.dialogueFinished = false;
        CompleteCurrentTask();
        Debug.Log($"[TaskManager] OnConditionSatisfied. complete task: {CurrentTask.ID}");
    }

    private IEnumerator ShowLevelUp()
    {
        levelUpUI.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(2f);
    }

    private IEnumerator CheckDebrisCount(int count)
    {
        _debrisCount = 0;
        yield return YieldInstructionCache.WaitUntil(() => _debrisCount == count);
    }

    [Button]
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

    public IEnumerator DoStartActions(string startActionStr) // instant
    {
        var values = startActionStr.Trim('(', ')', ' ').Replace(" ", string.Empty).Split(',');
        Debug.Log($"[TaskManager] DoStartActions: {values[0]}");
        if (values.Length == 0)
            yield break;

        switch (values[0])
        {
            case "spawnplanet":
                if (PlanetSpawner.Instance == default)
                    Debug.Log("This task requires a planet spawner.");
                else
                    PlanetSpawner.Instance.SpawnPlanet((PlanetType)Enum.Parse(typeof(PlanetType), values[1]));
                break;
            case "activatetutorialarrows":
                yield return YieldInstructionCache.WaitUntil(() => TutorialArrows.Instance != default);
                TutorialArrows.Activate(true);
                break;
            case "deactivatetutorialarrows":
                yield return YieldInstructionCache.WaitUntil(() => TutorialArrows.Instance != default);
                TutorialArrows.Activate(false);
                break;
        }
    }

    public IEnumerator RunEndActions(Action onDone)
    {
        foreach (var endAction in CurrentTask.EndAction.SeparateAllParenthesis())
        {
            var values = endAction.Trim('(', ')', ' ').Replace(" ", string.Empty).Split(',');
            if (values.Length == 0)
                yield break;

            yield return DoAction(values);
        }

        // yield return new WaitUntil(() => CloseScreen.Instance.IsClosing == false);
        onDone?.Invoke();
    }

    private IEnumerator DoAction(IReadOnlyList<string> values)
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
    }

    private IEnumerator ShowTaskComplete()
    {
        if (taskCompleteUI == default)
        {
            Debug.LogError("taskCompleteUI is null.");
            yield break;
        }
        yield return YieldInstructionCache.WaitUntil(() => taskCompleteUI.activeSelf == false);
        taskCompleteUI.GetComponent<TaskCompleteUI>().SetText(CurrentTask.Title);
        SoundService.PlaySfx("Assets/Audio/Capture Success.ogg", transform.position);
        taskCompleteUI.SetActive(true);
        yield return YieldInstructionCache.WaitForSeconds(2f);
    }
}