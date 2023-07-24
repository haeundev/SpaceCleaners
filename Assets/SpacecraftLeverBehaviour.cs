using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftLeverBehaviour : MonoBehaviour
{
    [SerializeField] private string sfxBoost = "Audio/Simple Boost.wav";
    
    private Transform _playerTransform;
    private SpacePlayer _spacePlayer;
    private XRLever _lever;
    private bool _isLeverUp;
    private bool _lastLeverState;

    private void Awake()
    {
        _lever = gameObject.GetComponent<XRLever>();
        var playerObj = GameObject.FindWithTag("Player");
        _playerTransform = playerObj.transform;
        _spacePlayer = playerObj.GetComponent<SpacePlayer>();
    }

    private void Start()
    {
        _lastLeverState = _isLeverUp;
    }

    private void Update()
    {
        _isLeverUp = _lever.value;

        if (_lastLeverState != _isLeverUp)
        {
            OuterSpaceEvent.Trigger_Boost(_isLeverUp);
            _lastLeverState = _isLeverUp;
            PlaySFX(_isLeverUp);
        }

        if (_isLeverUp || Input.GetKey(KeyCode.W))
        {
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.FastMoveSpeed);
        }
        else
        {
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.IdleMoveSpeed);
        }

    }

    private void PlaySFX(bool isLeverUp)
    {
        if (isLeverUp)
        {
            SoundService.PlaySfx(sfxBoost, transform.position);
        }
    }
}