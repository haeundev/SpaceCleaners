using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    private GrappleEffect _grappleEffect;
    private Camera _camera;
    [SerializeField] private InputActionProperty button;

    private void Awake()
    {
        _camera = Camera.main;
        _grappleEffect = GetComponent<GrappleEffect>();
    }

    private void Update()
    {
        if (button.action.WasPressedThisFrame())
        {
            var ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.cyan, .1f);
            if (Physics.Raycast(ray, out var info, 100/*, 1 << LayerMask.NameToLayer("Asteroid")*/))
                Debug.Log($"hit {info.collider.name}");
            if (info.collider == null)
                return;
            
            var hitObj = info.collider.gameObject;
            _grappleEffect.transform.LookAt(info.point);
            _grappleEffect.DoGrapple();
            
            Debug.Log("Do Grapple");
        }
        else if (button.action.WasReleasedThisFrame())
        {
            _grappleEffect.StopGrapple();
            Debug.Log("Stop Grapple");
        }
    }
}