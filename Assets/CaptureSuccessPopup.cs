using System;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;

public class CaptureSuccessPopup : MonoBehaviour
{
    [SerializeField] private Transform particleParent;
    [SerializeField] private string captureSuccessSFX = "Assets/Audio/Capture Success.ogg";
    
    private void Awake()
    {
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
    }

    private void OnDebrisCaptured(DebrisType _, GameObject __)
    {
        gameObject.SetActive(true);
        // Addressables.InstantiateAsync(captureSuccessSFX).Completed += op =>
        // {
        //     op.Result.gameObject.transform.position = particleParent.position;
        // };
        SpaceObjectLabels.DisableAll();
        SoundService.PlaySfx(captureSuccessSFX, transform.position);
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => { gameObject.SetActive(false); }).AddTo(this);
    }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnDebrisCaptured -= OnDebrisCaptured;
    }
}