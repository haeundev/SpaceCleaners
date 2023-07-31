using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class MonumentMonster : Monster
{
    [SerializeField] private Animator animator;
    private static readonly int AttackAnim = Animator.StringToHash("Attack"); // this is better than using string every time

    private void Start()
    {
        StartCoroutine(PlayAttack());

        // this will execute SetTrigger after 3 seconds
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => { animator.SetTrigger(AttackAnim); });
    }

    private IEnumerator PlayAttack()
    {
        yield return new WaitForSeconds(3f);
        Attack();
    }

    // call this function when monster should attack player
    private void Attack()
    {
        animator.SetTrigger(AttackAnim);
        // play sfx, etc.
    }

    public void SomeFunctionCalledAfterInstantiate()
    {
    }
}