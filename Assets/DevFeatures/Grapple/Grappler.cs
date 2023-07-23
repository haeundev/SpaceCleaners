using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    [SerializeField] private Transform drawDestination;
    [SerializeField] private InputActionProperty button;
    [SerializeField] private float drawSpeed = 17f;
    [SerializeField] private float grappleDoneDistance = 1f;
    [SerializeField] private float scaleShrinkBy = 0.25f;
    private IDisposable _disposable;
    private GrappleEffect _grappleEffect;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
        _grappleEffect = GetComponent<GrappleEffect>();
    }

    private void Update()
    {
        if (button.action.WasPressedThisFrame())
        {
            DoGrapple();
        }
        else if (button.action.WasReleasedThisFrame())
        {
            OnGrappleDone();
        }
    }

    public void DoGrapple()
    {
        var ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.cyan, .1f);
        var grappleTargetLayer = 1 << LayerMask.NameToLayer("GrappleTarget");
        var asteroidLayer = 1 << LayerMask.NameToLayer("Asteroid");
        var finalLayers = grappleTargetLayer | asteroidLayer;
        if (Physics.Raycast(ray, out var info, 100, finalLayers))
            Debug.Log($"hit {info.collider.name}");
        if (info.collider == null)
            return;
        
        var hitObj = info.collider.gameObject;
        _grappleEffect.transform.LookAt(info.point);
        _grappleEffect.DoGrapple();
        
        Debug.Log("Do Grapple");
        
        _disposable = Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
        {
            StartCoroutine(DrawTarget(hitObj));
        });
    }

    public void OnGrappleDone()
    {
        _disposable?.Dispose();
        _grappleEffect.StopGrapple();
        Debug.Log("Stop Grapple");
    }

    private IEnumerator DrawTarget(GameObject hitObj)
    {
        var shrinkV3 = new Vector3(scaleShrinkBy, scaleShrinkBy, scaleShrinkBy);
        _grappleEffect.StopGrapple();

        hitObj.GetComponentInChildren<Collider>().enabled = false;
        
        var dir = (drawDestination.position - hitObj.transform.position).normalized;
        var distance = Vector3.Distance(hitObj.transform.position, drawDestination.position);
        while (distance > grappleDoneDistance)
        {
            if (hitObj.transform.localScale.x > 0)
            {
                if (hitObj.transform.localScale.x - shrinkV3.x < 0)
                {
                    hitObj.transform.localScale = Vector3.zero;
                }
                else
                {
                    hitObj.transform.localScale -= shrinkV3;
                }
            }
            
            hitObj.transform.position += dir * (Time.deltaTime * drawSpeed);// The step size is equal to speed times frame time.
            distance = Vector3.Distance(hitObj.transform.position, drawDestination.position);
            yield return default;
        }
        // Add to inventory?
        Destroy(hitObj);
        Debug.Log($"User gets {hitObj.name}");
    }
}