using System;
using System.Collections;
using LiveLarson.Util;
using TMPro;
using UnityEngine;
using Random = System.Random;

public enum NotificationType
{
    Call,
    PlanetAlarm,
    AsteroidAlarm,
    ItemAlarm,
}


public class NotificationText : MonoBehaviour
{
    private TextMeshPro _tmp;

    private void Awake()
    {
        OuterSpaceEvent.NotificationReceived += OnNotificationReceived;
        _tmp = gameObject.GetComponent<TextMeshPro>();
        
        // test
        StartCoroutine(TestLoopNotifications());
    }

    private IEnumerator TestLoopNotifications()
    {
        while (enabled)
        {
            yield return YieldInstructionCache.WaitForSeconds(3f);
            var values = Enum.GetValues(typeof(NotificationType));
            var random = new Random();
            var type = (NotificationType)values.GetValue(random.Next(values.Length));
            OnNotificationReceived(type);
        }
    }

    private void OnNotificationReceived(NotificationType type)
    {
        _tmp.text = type switch
        {
            NotificationType.Call => "Receiving Call...",
            NotificationType.PlanetAlarm => "Planet Ahead",
            NotificationType.AsteroidAlarm => "Asteroid Ahead",
            NotificationType.ItemAlarm => "Item Ahead",
            _ => "Check Alarm"
        };
    }
}
