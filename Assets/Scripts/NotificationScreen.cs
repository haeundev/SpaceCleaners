using System;
using TMPro;
using UnityEngine;

public class NotificationScreen : MonoBehaviour
{
    private TextMeshPro _tmp;
    private MeshRenderer _meshRenderer;
    public ScreenPosition screenPosition;
    [SerializeField] private Material normalMat;
    [SerializeField] private Material cautionMat;
    [SerializeField] private Material warningMat;

    private void Awake()
    {
        OuterSpaceEvent.OnNotification += OnNotificationReceived;
        _tmp = gameObject.GetComponent<TextMeshPro>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void OnNotificationReceived(AlarmPriority alarmPriority, AlarmSourceType sourceType)
    {
        _tmp.text = sourceType switch
        {
            AlarmSourceType.Call => "Receiving Call...",
            AlarmSourceType.Planet => "Planet Ahead",
            AlarmSourceType.Asteroid => "Asteroid Ahead",
            AlarmSourceType.Item => "Item Ahead",
            _ => "Check Alarm"
        };

        PlaySFX(sourceType);
        MatchColor(alarmPriority);
    }

    private void MatchColor(AlarmPriority alarmPriority)
    {
        _meshRenderer.sharedMaterial = alarmPriority switch
        {
            AlarmPriority.Caution => warningMat,
            AlarmPriority.Warning => cautionMat,
            _ => normalMat
        };
    }

    private void PlaySFX(AlarmSourceType sourceType)
    {
        // HEO TODO
    }

    public void SetText(string text)
    {
        _tmp.text = text;
    }

    public void ResetMat()
    {
        _meshRenderer.sharedMaterial = normalMat;
    }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnNotification -= OnNotificationReceived;
    }
}