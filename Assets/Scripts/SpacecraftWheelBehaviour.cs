using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class SpacecraftWheelBehaviour : MonoBehaviour
{
    private Transform _playerTransform;
    private SpacePlayer _spacePlayer;
    private XRKnob _wheel;
    private float _wheelValue; // between -1 ~ 1
    private bool _isWheelActivated;
    
    private void Awake()
    {
        _wheel = gameObject.GetComponent<XRKnob>();
        var playerObj = GameObject.FindWithTag("Player");
        _playerTransform = playerObj.transform;
        _spacePlayer = playerObj.GetComponent<SpacePlayer>();
        
        _wheel.selectEntered.AddListener(OnWheelActivated);
        _wheel.selectExited.AddListener(OnWheelDectivated);
    }

    private void OnWheelActivated(SelectEnterEventArgs _)
    {
        _isWheelActivated = true;
    }

    private void OnWheelDectivated(SelectExitEventArgs _)
    {
        _isWheelActivated = false;
        _wheel.value = 0.5f;
    }

    private void Update()
    {
        _wheelValue = _wheel.value * 2 - 1;

        if (_isWheelActivated == false)
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.A))
            {
                _playerTransform.Rotate(Vector3.up * (_spacePlayer.RotateSpeed * -1 * Time.deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _playerTransform.Rotate(Vector3.up * (_spacePlayer.RotateSpeed * 1 * Time.deltaTime));
            }
#endif
            return;
        }
        
        _playerTransform.Rotate(Vector3.up * (_spacePlayer.RotateSpeed * _wheelValue * Time.deltaTime));
    }
}