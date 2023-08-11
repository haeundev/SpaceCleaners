using System;
using Febucci.UI;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;

public class TypewriterSoundPlayer : MonoBehaviour
{
    [SerializeField] private string sfxName = "Assets/Audio/DialogueBipSound.mp3";
    private Audio _currentAudio;
    private TypewriterByCharacter _typewriterByCharacter;

    private void Awake()
    {
        _typewriterByCharacter = gameObject.GetComponent<TypewriterByCharacter>();
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            _typewriterByCharacter.onTypewriterStart.AddListener(() =>
            {
                _currentAudio = SoundService.PlaySfx(sfxName, transform.position);
            });
            
            _typewriterByCharacter.onTextShowed.AddListener(() =>
            {
                _currentAudio?.Stop();
            });
            
            _typewriterByCharacter.onTextForceStopped.AddListener(() =>
            {
                _currentAudio?.Stop();
            });
        });
    }
}