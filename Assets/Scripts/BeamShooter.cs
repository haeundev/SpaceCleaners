using UnityEngine;
using UnityEngine.InputSystem;

public class BeamShooter : MonoBehaviour
{
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private float beamSpeed = 300f;
    [SerializeField] private float destroySec = 2f;
    [SerializeField] private InputActionProperty button;

    private void Update()
    {
        if (button.action.WasPressedThisFrame())
            ShootRay();
    }

    private void ShootRay()
    {
        Debug.Log("SHOOOOOOT!!!");
        
        var laser = Instantiate(shotPrefab, transform.position, transform.rotation);
        laser.GetComponent<ShotBehavior>().Init(beamSpeed, destroySec);
    }
}