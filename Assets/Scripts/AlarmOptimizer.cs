using System.Collections;
using System.Collections.Generic;
using LiveLarson.Util;
using UnityEngine;

public class AlarmOptimizer : MonoBehaviour
{
    private readonly List<AlarmByDistance> _activatedAlarms = new();
    private GameObject _newHit;
    private GameObject _lastHit;
    [SerializeField] private int rayCount = 15; // Number of rays to cast
    [SerializeField] private float rayDistance = 1000f; // The distance to cast the rays

    private void OnEnable()
    {
        StartCoroutine(RegularAllOff());
    }

    private IEnumerator RegularAllOff()
    {
        while (enabled)
        {
            yield return YieldInstructionCache.WaitForSeconds(10f);
            _activatedAlarms.ForEach(p => p.enabled = false);
            _activatedAlarms.Clear();
        }
    }

    private void Update()
    {
        // Create rays evenly distributed in a circle
        for (var i = 0; i < rayCount; i++)
        {
            var angle = i * 360f / rayCount;
            var direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            var ray = new Ray(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan, .1f);
            
            var asteroidLayer = 1 << LayerMask.NameToLayer("Asteroid");
            var planetLayer = 1 << LayerMask.NameToLayer("Planet");
            var finalLayers = asteroidLayer | planetLayer;
            if (Physics.Raycast(ray, out var info, rayDistance, finalLayers))
            {
                var alarm = info.collider.gameObject.GetComponent<AlarmByDistance>();
                if (alarm != default)
                {
                    alarm.enabled = true;
                    _activatedAlarms.Add(alarm);
                }
            }
        }
    }
}