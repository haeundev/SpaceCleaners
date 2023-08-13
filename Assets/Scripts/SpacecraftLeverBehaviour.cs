using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftLeverBehaviour : MonoBehaviour
{
    [SerializeField] private string sfxBoost = "Assets/Audio/Simple Boost.wav";
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float chromaticAmount = 0.1f;

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
            // is boosting
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.FastMoveSpeed);

            if (Chromatic.intensity.value < 0.5f)
            {
                Chromatic.intensity.value += Time.deltaTime * chromaticAmount;
            }
        }
        else
        {
            _playerTransform.position += _playerTransform.forward * (Time.deltaTime * _spacePlayer.IdleMoveSpeed);

            if (Chromatic.intensity.value > 0f)
            {
                Chromatic.intensity.value -= Time.deltaTime * chromaticAmount;
            }
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