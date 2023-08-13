using System;
using System.Collections.Generic;
using GogoGaga.UHM;
using LiveLarson.SoundSystem;
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

    public MonsterHUD monsterHUD;
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
                _health = 2;
                monsterHUD.SetSliderMaxValue(_health);
                break;
            case MonsterItemType.Badge:
                _health = 4;
                goByType[itemType].SetActive(true);
                monsterHUD.SetSliderMaxValue(_health);
                break;
            case MonsterItemType.Mustache:
                _health = 8;
                goByType[itemType].SetActive(true);
                monsterHUD.SetSliderMaxValue(_health);
                break;
            case MonsterItemType.Wig:
                _health = 10;
                goByType[itemType].SetActive(true);
                monsterHUD.SetSliderMaxValue(_health);
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Default"))
            return;
        
        print(other.gameObject.name);
        if (_health > 0)
        {
            _health--;
            monsterHUD.MonsterTakeDamage(1);
            if (_health == 0)
            {
                Destroy(gameObject);
            }
        }
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

    public void OnPlayerNear(GameObject player)
    {
        Attack(player);
    }

    private void Attack(GameObject player)
    {
        _animator.SetTrigger(AttackAnim);
        // SoundService.PlaySfx("~~~", transform.position);
        player.GetComponentInChildren<MonumentPlayer>().OnAttacked();
    }
}