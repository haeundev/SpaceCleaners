using UnityEngine;

public class RepositionOnStart : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void OnEnable()
    {
        transform.position = target.position;
    }
}