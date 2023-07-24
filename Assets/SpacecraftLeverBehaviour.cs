using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftLeverBehaviour : MonoBehaviour
{
    private Transform _playerTransform;
    private SpacePlayer _spacePlayer;
    private XRLever _lever;
    private bool _isLeverUp;

    private void Awake()
    {
        _lever = gameObject.GetComponent<XRLever>();
        var playerObj = GameObject.FindWithTag("Player");
        _playerTransform = playerObj.transform;
        _spacePlayer = playerObj.GetComponent<SpacePlayer>();
    }

    private void Update()
    {
        _isLeverUp = _lever.value;

        if (_isLeverUp || Input.GetKey(KeyCode.W))
        {
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.FastMoveSpeed);
            OuterSpaceEvent.Trigger_Boost(true);
        }
        else
        {
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.IdleMoveSpeed);
            OuterSpaceEvent.Trigger_Boost(false);
        }
    }
}