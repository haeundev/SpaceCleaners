using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActivateInSeconds : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToActivate;
    [SerializeField] private float seconds;
    private IDisposable _disposable;
    
    private void Start()
    {
        _disposable = Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe(_ =>
        {
            objectsToActivate.ForEach(p => p.SetActive(true));
        }).AddTo(this);
    }

    private void OnDestroy()
    {
        _disposable?.Dispose();
    }
}