using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using Pathfinding;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public enum MonsterState
{
    Idle,
    Run,
    Attack,
    Die
}

public class MonumentMonster : Monster
{
    [SerializeField] private List<AssetReference> dropItems;
    [SerializeField] private List<string> sfxOnGetHit;
    [SerializeField] private List<string> sfxOnDie;
    private MonsterState _state = MonsterState.Idle;
    public MonsterHUD monsterHUD;
    private int _health;
    private Animator _animator;
    private MonsterSpawnPositions _spawnPositions;
    private AIDestinationSetter _aiDestinationSetter;
    
    private static readonly List<int> AttackAnims = new()
    {
        Animator.StringToHash("PunchForward"), Animator.StringToHash("PunchUpward"), Animator.StringToHash("Push")
    };
    private static readonly List<int> IdleAnims = new()
    {
        Animator.StringToHash("Idle"), Animator.StringToHash("Jump"), Animator.StringToHash("Dance"), Animator.StringToHash("Victory")
    };
    private static readonly List<int> RunAnims = new()
    {
        Animator.StringToHash("Run"), Animator.StringToHash("Slide")
    };
    private static readonly int DieAnim = Animator.StringToHash("Die");
    
    private AIPath _aiPath;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _aiDestinationSetter = GetComponentInChildren<AIDestinationSetter>();
        _aiPath = GetComponentInChildren<AIPath>();
    }

    private void Start()
    {
        _spawnPositions = FindObjectOfType<MonsterSpawnPositions>();
        _health = 10;
        monsterHUD.SetSliderMaxValue(_health);
        ChangeState(MonsterState.Idle);
    }
    
    public void OnGetHit()
    {
        if (_health > 0)
        {
            SoundService.PlaySfx(sfxOnGetHit.PeekRandom(), transform.position);
            _health--;
            monsterHUD.MonsterTakeDamage(1);
            if (_health <= 0)
            {
                ChangeState(MonsterState.Die);
                Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                {
                    Destroy(gameObject);
                }).AddTo(this);
            }
        }
    }

    private void ChangeState(MonsterState targetState)
    {
        _state = targetState;
        StopAllCoroutines();
        
        switch (targetState)
        {
            case MonsterState.Idle:
                _animator.SetTrigger(IdleAnims.PeekRandom());
                StartCoroutine(IdleRoutine());
                break;
            case MonsterState.Run:
                _animator.SetTrigger(RunAnims.PeekRandom());
                break;
            case MonsterState.Attack:
                _animator.SetTrigger(AttackAnims.PeekRandom());
                break;
            case MonsterState.Die:
                _animator.SetTrigger(DieAnim);
                break;
        }
    }

    public void OnPlayerEnterFollowProxy(GameObject player)
    {
        ChangeState(MonsterState.Run);
    }

    public void OnPlayerExitFollowProxy(GameObject player)
    {
        ChangeState(MonsterState.Idle);
    }

    private IEnumerator IdleRoutine()
    {
        while (_state == MonsterState.Idle)
        {
            yield return YieldInstructionCache.WaitForSeconds(Random.Range(1, 10));
            if (_state != MonsterState.Idle)
            {
                yield break;
            }
            _aiPath.enabled = true;
            var randomPos = _spawnPositions.positions.PeekRandom();
            _aiDestinationSetter.target = randomPos;
        }
    }
    
    private IEnumerator AttackRoutine()
    {
        yield break;
        // while (_state == MonsterState.Attack)
        // {
        //     
        // }
    }
    
    public void OnPlayerEnterAttackProxy(GameObject player)
    {
        ChangeState(MonsterState.Attack);
        player.GetComponentInChildren<MonumentPlayer>().OnAttacked();
    }
    
    public void OnPlayerExitAttackProxy(GameObject player)
    {
        ChangeState(MonsterState.Run);
    }
}