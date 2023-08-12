using System;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterLevelType
{
    Minion,
    Boss
}

public enum MonsterItemType
{
    None,
    Badge,
    Mustache,
    Wig,
}

[Serializable]
public class MonsterTypeDictionary : SerializableDictionary<MonsterItemType, GameObject> {}

public class MonumentMonster : Monster
{
    [SerializeField] private MonsterLevelType levelType;
    [SerializeField] private MonsterItemType itemType;
    [SerializeField] private MonsterTypeDictionary goByType;
    
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
    
    public void SetItemType(MonsterItemType monsterItemType)
    {
        itemType = monsterItemType;
        ApplyItemType();
    }

    public void SetLevelType(MonsterLevelType monsterLevelType)
    {
        levelType = monsterLevelType;
        ApplyLevelType();
    }

    private void ApplyLevelType()
    {
        if (levelType == MonsterLevelType.Boss)
        {
            transform.localScale *= 3f;
        }
    }

    private void ApplyItemType()
    {
        switch (itemType)
        {
            case MonsterItemType.None:
                break;
            case MonsterItemType.Badge:
            case MonsterItemType.Mustache:
            case MonsterItemType.Wig:
                goByType[itemType].SetActive(true);
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
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