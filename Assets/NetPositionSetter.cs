using UnityEngine;

public class NetPositionSetter : MonoBehaviour
{
    [SerializeField] private Transform netHead;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform targetTransform;

    private void Start()
    {
        transform.LookAt(targetTransform);
        transform.position = netHead.position + offset;
    }
}