using System;
using Febucci.UI;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;

public class TypewriterSoundPlayer : MonoBehaviour
{
    private string _sfxName = "Assets/Audio/DialogueBipSound.mp3";
    private Audio _currentAudio;
    private TypewriterByCharacter _typewriterByCharacter;

    private void Awake()
    {
        _typewriterByCharacter = gameObject.GetComponent<TypewriterByCharacter>();
        
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            _typewriterByCharacter.onTypewriterStart.AddListener(() =>
            {
                OnStart();
            });
            
            _typewriterByCharacter.onTextShowed.AddListener(() =>
            {
                OnEnd();
            });
            
            _typewriterByCharacter.onTextForceStopped.AddListener(() =>
            {
                OnEnd();
            });
        });
    }

    private void OnStart()
    {
        _currentAudio = SoundService.PlaySfx(_sfxName, transform.position);
    }
    
    private void OnEnd()
    {
        _currentAudio?.Stop();
    }

    public void SetSFX(string audioName)
    {
        _sfxName = audioName;
    }
}