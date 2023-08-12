using System;
using System.Collections;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class MonumentMonster : Monster
{
    private int _health;
    private Animator _animator;
    
    private static readonly int AttackAnim = Animator.StringToHash("Attack"); // this is better than using string every time
    private static readonly int IdleAnim = Animator.StringToHash("Idle"); // this is better than using string every time
    private static readonly int DieAnim = Animator.StringToHash("Die"); // this is better than using string every time
    private static readonly int WalkAnim = Animator.StringToHash("Walk"); 

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Weapon"))
            return;
        

    }



    // private void Start()
    // {
    //     StartCoroutine(PlayAttack());

    //     // this will execute SetTrigger after 3 seconds
    //     Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => { _animator.SetTrigger(AttackAnim); });
    // }

    // private IEnumerator PlayAttack()
    // {
    //     yield return new WaitForSeconds(3f);
    //     Attack();
    // }

    // // call this function when monster should attack player
    // private void Attack()
    // {
    //     animator.SetTrigger(AttackAnim);
    //     // play sfx, etc.
    // }

    // public void SomeFunctionCalledAfterInstantiate()
    // {
    // }
}