using System;
using UniRx;
using UnityEngine;

public class DestroyOnParticleFinish : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        Observable.Timer(TimeSpan.FromSeconds(_particleSystem.main.duration)).Subscribe(_ => Destroy(gameObject));
    }
}