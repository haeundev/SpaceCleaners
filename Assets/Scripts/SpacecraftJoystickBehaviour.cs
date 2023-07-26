using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftJoystickBehaviour : MonoBehaviour
{
    private Transform _playerTransform;
    private SpacePlayer _spacePlayer;
    private XRJoystick _joystick;
    [SerializeField] private float mitigateForSickness = 0.3f;

    private void Awake()
    {
        _joystick = gameObject.GetComponent<XRJoystick>();
        var playerObj = GameObject.FindWithTag("Player");
        _playerTransform = playerObj.transform;
        _spacePlayer = playerObj.GetComponent<SpacePlayer>();
    }

    private void Update()
    {
        _playerTransform.Rotate(Vector3.forward * (_joystick.value.x * mitigateForSickness * (_spacePlayer.RotateSpeed * -1 * Time.deltaTime)));
        _playerTransform.Rotate(-Vector3.right * (_joystick.value.y * mitigateForSickness * (_spacePlayer.RotateSpeed * -1 * Time.deltaTime)));
    }
}