using System.Collections;
using System.Linq;
using DataTables;
using LiveLarson.DataTableManagement;
using LiveLarson.Util;
using UnityEngine;

public enum ScreenPosition
{
    Left,
    Right
}

public class NotificationManager : MonoBehaviour
{
    private NotificationScreen[] _screens;
    private NotificationScreen _leftScreen;
    private NotificationScreen _rightScreen;
    [SerializeField] private Notifications notifications;

    private void Awake()
    {
        OuterSpaceEvent.OnNotification += OnNotificationReceived;
        _screens = FindObjectsOfType<NotificationScreen>();
        _leftScreen = _screens.Single(p => p.screenPosition == ScreenPosition.Left);
        _rightScreen = _screens.Single(p => p.screenPosition == ScreenPosition.Right);
    }

    private void Start()
    {
        StartCoroutine(LoopDefault());
    }

    private IEnumerator LoopDefault()
    {
        while (enabled)
        {
            yield return YieldInstructionCache.WaitForSeconds(10f);
            var data = notifications.Values.PeekRandom();
            Debug.Log($"Notification Default: {data.Value}");
            var text = data.Value.SplitIntoTwo();
            if (text.Length != 2)
                yield break;
            _leftScreen.SetText(text[0]);
            _rightScreen.SetText(text[1]);
        }
    }

    private void OnNotificationReceived(AlarmPriority alarmPriority, AlarmSourceType sourceType)
    {
        StopAllCoroutines();
        
        if (sourceType == AlarmSourceType.Off)
        {
            _screens.ForEach(p => p.ResetMat());

            StartCoroutine(LoopDefault());
            return;
        }

        _screens.ForEach(p => p.OnNotificationReceived(alarmPriority, sourceType));
    }
}