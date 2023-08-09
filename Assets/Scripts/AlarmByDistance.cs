using System.Collections;
using LiveLarson.Util;
using UnityEngine;

public enum AlarmSourceType
{
    Off, // turns alarm off
    Call,
    Asteroid, // 80
    Planet, // 2550
    Item
}

public class AlarmByDistance : MonoBehaviour
{
    [SerializeField] private AlarmSourceType type;
    [SerializeField] private int cautionDistance;
    [SerializeField] private int warningDistance;
    [SerializeField] private int forwardAngle = 90;
    private Transform _playerTransform;
    private bool _isAlarmTriggered;

    private void Awake()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
        
        // to coroutine
        StartCoroutine(CheckForAlarm());
    }

    private IEnumerator CheckForAlarm()
    {
        while (enabled)
        {
            if (Vector3.Angle(_playerTransform.forward, transform.position - _playerTransform.position) < forwardAngle)
            {
                if (Vector3.Distance(transform.position, _playerTransform.position) < warningDistance)
                {
                    OuterSpaceEvent.Trigger_Notification(AlarmPriority.Warning, type);
                    enabled = false;
                }
                else if (Vector3.Distance(transform.position, _playerTransform.position) < cautionDistance)
                {
                    OuterSpaceEvent.Trigger_Notification(AlarmPriority.Caution, type);
                    enabled = false;
                }
            }

            yield return YieldInstructionCache.WaitForSeconds(10f);
        }
    }
}