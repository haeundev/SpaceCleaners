using System;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;

public class CaptureSuccessPopup : MonoBehaviour
{
    private void Awake()
    {
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
    }

    private void OnDebrisCaptured(GameObject _)
    {
        gameObject.SetActive(true);
        SoundService.PlaySfx("Assets/Audio/Level Up.wav", transform.position);
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => { gameObject.SetActive(false); }).AddTo(this);
    }
}