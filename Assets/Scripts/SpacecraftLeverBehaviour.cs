using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftLeverBehaviour : MonoBehaviour
{
    [SerializeField] private string sfxBoostOn = "Assets/Audio/swoosh-on.mp3";
    [SerializeField] private string sfxBoostOff = "Assets/Audio/swoosh-off.mp3";
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float chromaticAmount = 0.1f;
    [SerializeField] private float accelerationRate = 0.95f;

    private ChromaticAberration _chromatic;
    private ChromaticAberration Chromatic
    {
        get
        {
            if (_chromatic != default) return _chromatic;
            if (globalVolume.profile.TryGet<ChromaticAberration>(out var chrome))
                _chromatic = chrome;
            return _chromatic;
        }
    }

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

#if UNITY_EDITOR
    public void ToggleLever()
    {
        _lever.value = !_lever.value;
    }
#endif
    
    private float _currentSpeed; // Variable to keep track of the current speed

    private void Update()
    {
        _isLeverUp = _lever.value;

        if (_lastLeverState != _isLeverUp)
        {
            OuterSpaceEvent.Trigger_Boost(_isLeverUp);
            _lastLeverState = _isLeverUp;
            PlaySFX(_isLeverUp);
        }

        float targetSpeed = _isLeverUp || Input.GetKey(KeyCode.W) ? _spacePlayer.FastMoveSpeed : _spacePlayer.IdleMoveSpeed;

        // Gradually adjust the current speed towards the target speed
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * accelerationRate);

        // Apply the current speed to the player's position
        _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _currentSpeed);

        // Adjust Chromatic intensity based on the lever state
        if (_isLeverUp)
        {
            if (Chromatic.intensity.value < 0.5f)
            {
                Chromatic.intensity.value += Time.deltaTime * chromaticAmount;
            }
        }
        else
        {
            if (Chromatic.intensity.value > 0f)
            {
                Chromatic.intensity.value -= Time.deltaTime * chromaticAmount;
            }
        }
    }

    private void PlaySFX(bool isLeverUp)
    {
        SoundService.PlaySfx(isLeverUp ? sfxBoostOn : sfxBoostOff, transform.position);
    }
}