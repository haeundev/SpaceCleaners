using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private float speed = 35f;

    private void Update()
    {
        transform.Rotate(Vector3.up * (speed * Time.deltaTime));
    }
}