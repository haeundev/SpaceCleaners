using System;
using LiveLarson.SoundSystem;
using UnityEngine;

public class MonsterAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private string attackSfx = "Assets/Audio/MonsterAttack.mp3";
    [SerializeField] private GameObject soulEscape;
    [SerializeField] private GameObject explosionSmall;
    [SerializeField] private GameObject explosionBig;

    private void Awake()
    {
        soulEscape.SetActive(false);
        explosionSmall.SetActive(false);
        explosionBig.SetActive(false);
    }

    public void OnAttack()
    {
        Debug.Log($"[AnimationEventHandler] OnAttack");
        SoundService.PlaySfx(attackSfx, transform.position);
    }
    
    public void OnHit()
    {
        Debug.Log($"[AnimationEventHandler] OnHit");
        explosionSmall.SetActive(true);
    }

    public void OnDie()
    {
        Debug.Log($"[AnimationEventHandler] OnDie");
        soulEscape.SetActive(true);
        explosionBig.SetActive(true);
    }
}