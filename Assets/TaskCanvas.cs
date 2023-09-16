using DataTables;
using LiveLarson.Enums;
using LiveLarson.SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskCanvas : MonoBehaviour
{
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpDetail;
    [SerializeField] private Button noTeleportButton;
    [SerializeField] private Button teleportButton;
    [SerializeField] private Transform playerTeleportPoint;

    private void Awake()
    {
        TaskManager.Instance.OnTaskAcquired += OnTaskAcquired;
        teleportButton.onClick.AddListener(OnTeleportButtonClicked);
        noTeleportButton.onClick.AddListener(OnNoTeleportButtonClicked);
    }

    private void Start()
    {
        tmpTitle.SetText("");
        tmpDetail.SetText("");
    }

    private void OnTeleportButtonClicked()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        playerObj.transform.position = playerTeleportPoint.position;
        playerObj.transform.rotation = playerTeleportPoint.rotation;
        gameObject.SetActive(false);
        SoundService.PlaySfx("Assets/Audio/Swoosh.mp3", transform.position);
    }

    private void OnNoTeleportButtonClicked()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (TaskManager.Instance == default)
        {
            Debug.LogError("TaskManager is not initialized");
            return;
        }
        if (TaskManager.Instance.CurrentTask == default)
        {
            Debug.LogError("CurrentTask is not initialized");
            return;
        }
        var taskInfo = TaskManager.Instance.CurrentTask;
        tmpTitle.SetText(taskInfo.Title);
        tmpDetail.SetText(taskInfo.ValueStr);
        noTeleportButton.gameObject.SetActive(taskInfo.TaskType == TaskType.GoToPlanet);
        teleportButton.gameObject.SetActive(taskInfo.TaskType == TaskType.GoToPlanet);
    }
    
    private void OnTaskAcquired(TaskInfo taskInfo)
    {
        // characterIcon.sprite = GetSprite()
    }
}