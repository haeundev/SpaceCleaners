using System;
using UniRx;
using UnityEngine;

public class BoxingGloveBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject powerEffect;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float hitEffectDuration = 1f;
    [SerializeField] private float powerEffectDuration = 3f;
    private bool _isHitEffectActive;
    private bool _isPowerEffectActive;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (_isHitEffectActive == false)
            {
                hitEffect.SetActive(true);
                _isHitEffectActive = true;
                Observable.Timer(TimeSpan.FromSeconds(hitEffectDuration)).Subscribe(_ =>
                {
                    hitEffect.SetActive(false);
                    _isHitEffectActive = false;
                });
            }
            
            if (_isPowerEffectActive == false)
            {
                powerEffect.SetActive(true);
                _isPowerEffectActive = true;
                Observable.Timer(TimeSpan.FromSeconds(powerEffectDuration)).Subscribe(_ =>
                {
                    powerEffect.SetActive(false);
                    _isPowerEffectActive = false;
                });
            }
        }
    }
}