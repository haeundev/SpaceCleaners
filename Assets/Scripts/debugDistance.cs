using UnityEngine;

public class debugDistance : MonoBehaviour
{
    [SerializeField] private Transform home;
    [SerializeField] private Transform target;

    private void Update()
    {
        if (home != default && target != default)
            Debug.Log($"Distance: {Vector3.Distance(home.position, target.position)}");
    }
}