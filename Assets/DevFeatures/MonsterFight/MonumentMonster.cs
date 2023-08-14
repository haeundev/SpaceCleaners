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
    Die,
    Attacked,
}

public class MonumentMonster : Monster
{
    [SerializeField] private List<AssetReference> dropItems;
    [SerializeField] private List<AssetReference> particlesOnHit;
    [SerializeField] private List<GameObject> particlesOnDie;
    [SerializeField] private List<string> sfxOnGetHit;
    [SerializeField] private List<string> sfxOnDie;
    [SerializeField] private float knockBackIntensity = 5f;
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
        Animator.StringToHash("Run")
    };
    private static readonly int DieAnim = Animator.StringToHash("Die");
    
    private AIPath _aiPath;
    private Transform _playerTransform;
    private bool _isKnockBack;

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
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private bool _hitCoolTimeDone = true;
    
    public void OnGetHit()
    {
        if (_hitCoolTimeDone == false)
        {
            return;
        }

        _hitCoolTimeDone = false;
        Observable.Timer(TimeSpan.FromSeconds(_hitCoolTime)).Subscribe(_ => _hitCoolTimeDone = true);

        ChangeState(MonsterState.Attack);
        
        if (_health > 0)
        {
            _isKnockBack = true;
            Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => _isKnockBack = false);
            SoundService.PlaySfx(sfxOnGetHit.PeekRandom(), transform.position);
            _health--;
            monsterHUD.MonsterTakeDamage(1);
            CreateParticleOnHit();
            _animator.SetTrigger(AttackAnims.PeekRandom());
            
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    private readonly float _hitCoolTime = 1f;
    
    private void CreateParticleOnHit()
    {
        var randomParticle = particlesOnHit.PeekRandom();
        Addressables.InstantiateAsync(randomParticle, transform.position, Quaternion.identity).Completed += handle =>
        {
            Debug.Log($"particle: {handle.Result}");
            var particle = handle.Result;
            particle.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(particle.GetComponentInChildren<ParticleSystem>().time)).Subscribe(_ =>
            {
                Addressables.ReleaseInstance(particle);
                Destroy(particle);
            });
        };
    }

    private void CreateParticleOnDie()
    {
        particlesOnDie.ForEach(p =>
        {
            p.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(p.GetComponentInChildren<ParticleSystem>().time)).Subscribe(_ =>
            {
                p.SetActive(false);
            });
        });
    }

    private void Die()
    {
        _aiPath.enabled = false;
        SoundService.PlaySfx(sfxOnDie.PeekRandom(), transform.position);
        CreateParticleOnDie();
        ChangeState(MonsterState.Die);
        Observable.Timer(TimeSpan.FromSeconds(2.1f)).Subscribe(_ =>
        {
            Destroy(gameObject);
        }).AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_isKnockBack)
        {
            var playerForward = _playerTransform.transform.forward;
            //var dir = new Vector3(playerForward.x, playerForward.y, playerForward.z);
            transform.position += (playerForward.normalized) * (Time.deltaTime * knockBackIntensity);
        }
    }

    private void ChangeState(MonsterState targetState)
    {
        _state = targetState;
        StopAllCoroutines();
        
        Debug.Log($"[MonumentMonster] Changed state to {_state}");
        
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
            yield return YieldInstructionCache.WaitForSeconds(Random.Range(5, 15));
            if (_state != MonsterState.Idle)
            {
                yield break;
            }
            _aiPath.enabled = true;
            var randomPos = _spawnPositions.positions.PeekRandom();
            _animator.SetTrigger(RunAnims.PeekRandom());
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