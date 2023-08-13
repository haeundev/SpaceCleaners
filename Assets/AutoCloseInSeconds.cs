using System;
using UniRx;
using UnityEngine;

public class AutoCloseInSeconds : MonoBehaviour
{
    [SerializeField] private float closeInSeconds = 5f;

    private void OnEnable()
    {
        Observable.Timer(TimeSpan.FromSeconds(closeInSeconds)).Subscribe(_ =>
        {
            gameObject.SetActive(false);
        }).AddTo(this);
    }
}