using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private float speed = 35f;
    [SerializeField] private bool changeRotateAxis;

    private void Update()
    {
        transform.Rotate(changeRotateAxis? Vector3.right : Vector3.up * (speed * Time.deltaTime));
    }
}