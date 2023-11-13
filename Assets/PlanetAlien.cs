using System;
using UniRx;
using UnityEngine;

public class PlanetAlien : MonoBehaviour
{
    private readonly int AnimVictory = Animator.StringToHash("Victory");
    private readonly int AnimDisappointed = Animator.StringToHash("Disappointed");

    [SerializeField] private GameObject particleCorrect;
    
    private Animator _animator;
    private IDisposable _pcDisposable;
    
    private void Awake()
    {
        particleCorrect.SetActive(false);
        MonumentEvents.OnRecycleWrong += OnRecycleWrong;
        MonumentEvents.OnRecycleCorrect += OnRecycleCorrect;
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnRecycleCorrect()
    {
        particleCorrect.SetActive(false);
        _pcDisposable?.Dispose();
        particleCorrect.SetActive(true);
        _pcDisposable = Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => particleCorrect.SetActive(false)).AddTo(this);
        
        _animator.SetTrigger(AnimVictory);
    }

    private void OnRecycleWrong()
    {
        _animator.SetTrigger(AnimDisappointed);
    }

    private void OnDestroy()
    {
        MonumentEvents.OnRecycleWrong -= OnRecycleWrong;
        MonumentEvents.OnRecycleCorrect -= OnRecycleCorrect;
    }
}
