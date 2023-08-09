using Sirenix.OdinInspector;
using UnityEngine;

public class RepositionOnStart : MonoBehaviour
{
    public Transform head;
    public Transform origin;
    public Transform target;

    private Vector3 _offset;
    private Vector3 _targetForward;
    private Vector3 _cameraForward;
    private float _angle;
    
    [Button]
    private void Start()
    {
        Recenter();
    }

    public void Recenter()
    {
        _offset = head.position - origin.position;
        _offset.y = 0;
        origin.position = target.position + _offset;
      
        _targetForward = target.forward;
        _targetForward.y = 0;
        _cameraForward = head.forward;
        _cameraForward.y = 0;

        _angle = Vector3.SignedAngle(_cameraForward, _targetForward, Vector3.up);
      
        origin.RotateAround(head.position, Vector3.up, _angle);
    }
    
    // [SerializeField] private Transform target;
    //
    // [Button]
    // private void OnEnable()
    // {
    //     transform.position = target.position;
    // }
}