using UnityEngine;

public class RayGaze : MonoBehaviour
{
    [SerializeField] private float gazeDistance = 500f;
    private GameObject _newHit;
    private GameObject _lastHit;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_camera == default 
            || SpaceObjectLabels.Instance == default
            || SpaceObjectLabels.Instance.labels == default
            || SpaceObjectLabels.Instance.labels.Count == 0
            )
            return;
        
        var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var lastGazable = _lastHit == default ? default : _lastHit.GetComponent<RayGazable>();
        var asteroidLayer = 1 << LayerMask.NameToLayer("Asteroid");
        var planetLayer = 1 << LayerMask.NameToLayer("Planet");
        var debrisLayer = 1 << LayerMask.NameToLayer("Debris");
        var finalLayers = asteroidLayer | planetLayer | debrisLayer;
        Debug.DrawRay(ray.origin, ray.direction * gazeDistance, Color.cyan, .1f);
        if (Physics.Raycast(ray, out var info, gazeDistance, finalLayers))
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
            if (newGazable == default)
            {
                Debug.LogError($"why is RayGazable null? {_newHit.name}");
                return;
            }

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

                //Debug.Log($"focus enter {_newHit.name}");
                newGazable.OnGazeEnter();
                FocusGauge.OnFocusEnter();
            }
        }
    }
}