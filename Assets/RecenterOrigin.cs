using UnityEngine;
using UnityEngine.InputSystem;

public class RecenterOrigin : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform target;
    [SerializeField] private InputActionProperty recenterButton;

    private void Start()
    {
        Recenter();
    }

    // public void Recenter()
    // {
    //     var xrOrigin = GetComponent<XROrigin>();
    //     xrOrigin.MoveCameraToWorldLocation(target.position);
    //     xrOrigin.MatchOriginUpCameraForward(target.up, target.forward);
    // }

    public void Recenter()
    {
        var offset = head.position - origin.position;
        offset.y = 0;
        origin.position = target.position - offset;

        var targetForward = target.forward;
        targetForward.y = 0;
        var cameraForward = head.forward;
        cameraForward.y = 0;

        var angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        origin.RotateAround(head.position, Vector3.up, angle);
    }

    // Update is called once per frame
    private void Update()
    {
        if (recenterButton.action.WasPressedThisFrame()) Recenter();
    }
}