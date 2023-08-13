using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine.InputSystem.XR;

public class CameraShakeTweener : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float strength = 0.2f;
    
    [Button]
    private void DoTween()
    {
        var tpd = gameObject.GetComponentInChildren<TrackedPoseDriver>();
        tpd.enabled = false;
        transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
        Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe(_ =>
        {
            tpd.enabled = true;
        });
    }
}