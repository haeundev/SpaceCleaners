using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGaze : MonoBehaviour
{
    private GameObject _newHit;
    private GameObject _lastHit;
    
    private void Update()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.cyan, .1f);
        if (Physics.Raycast(ray, out var info, Mathf.Infinity, 1 << LayerMask.NameToLayer("Asteroid")))
            Debug.Log($"hit {info.collider.name}");
        if (info.collider == null) return;
        
        _newHit = info.collider.gameObject;
        var newGazable = _newHit.GetComponent<RayGazable>();
        var lastGazable = _lastHit == default ? default : _lastHit.GetComponent<RayGazable>();

        if (_newHit == _lastHit)
        {
            newGazable.OnGazeStay();

        }
        else
        {
            _lastHit = _newHit;
            if (lastGazable != default)
                lastGazable.OnGazeExit();
            newGazable.OnGazeEnter();
        }
   
    }
}
