using System;
using UnityEngine;

public class RayGaze : MonoBehaviour
{
    private GameObject _newHit;
    private GameObject _lastHit;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_camera == default)
            return;
        
        var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var lastGazable = _lastHit == default ? default : _lastHit.GetComponent<RayGazable>();

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.cyan, .1f);
        if (Physics.Raycast(ray, out var info, Mathf.Infinity, 1 << LayerMask.NameToLayer("Asteroid")))
        {
            //Debug.Log($"hit {info.collider.name}");
        }
        if (info.collider == null)
        {
            if (lastGazable != default)
            {
                Debug.Log($"focus exit {lastGazable.gameObject.name}");
                lastGazable.OnGazeExit();
                FocusGauge.OnFocusExit();
                _lastHit = default;
            }
        }
        else
        {
            _newHit = info.collider.gameObject;
            var newGazable = _newHit.GetComponent<RayGazable>();

            if (_newHit == _lastHit)
            {
                newGazable.OnGazeStay();
            }
            else
            {
                _lastHit = _newHit;
                if (lastGazable != default)
                {
                    lastGazable.OnGazeExit();
                    FocusGauge.OnFocusExit();
                }
                
                Debug.Log($"focus enter {_newHit.name}");
                newGazable.OnGazeEnter();
                FocusGauge.OnFocusEnter();
            }
        }
    }
}