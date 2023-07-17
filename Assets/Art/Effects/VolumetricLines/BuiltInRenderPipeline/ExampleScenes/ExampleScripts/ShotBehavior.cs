using UnityEngine;

public class ShotBehavior : MonoBehaviour
{
    [SerializeField] private GameObject collisionExplosion;
    [SerializeField] private Vector3 target;
    private float _speed;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * _speed;// The step size is equal to speed times frame time.
    }

    public void Init(float speed, float destroySec)
    {
        _speed = speed;
        Destroy(gameObject, destroySec);
    }
}