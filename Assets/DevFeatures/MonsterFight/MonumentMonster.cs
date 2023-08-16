using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using Pathfinding;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MonsterState
{
    Idle,
    Run,
    Attack,
    Die,
    Attacked
}

public class MonumentMonster : MonoBehaviour
{
    [SerializeField] private GameObject fuelTankPrefab;
    [SerializeField] private List<GameObject> particlesOnHit;
    [SerializeField] private List<GameObject> particlesOnDie;
    [SerializeField] private List<string> sfxOnGetHit;
    [SerializeField] private List<string> sfxOnDie;
    [SerializeField] private List<GameObject> disableOnDie;
    [SerializeField] private float knockBackIntensity = 5f;
    [SerializeField] private string sfxTalk = "Assets/Audio/MonsterTalk.mp3";
    [SerializeField] private string sfxLaugh = "Assets/Audio/LittleMonsterLaugh.wav";
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
        Animator.StringToHash("Idle"), Animator.StringToHash("Jump"), Animator.StringToHash("Dance"),
        Animator.StringToHash("Victory")
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
    private int _hitCountBeforeKnockBack;

    public void OnGetHit()
    {
        _hitCountBeforeKnockBack++;

        //if (_hitCoolTimeDone == false)
        //{
        //    return;
        //}
        //
        //_hitCoolTimeDone = false;
        //Observable.Timer(TimeSpan.FromSeconds(_hitCoolTime)).Subscribe(_ => _hitCoolTimeDone = true);

        ChangeState(MonsterState.Attacked);

        if (_health > 0)
        {
            if (_isKnockBack == false && _hitCountBeforeKnockBack >= 3)
            {
                _isKnockBack = true;
                Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => _isKnockBack = false);
                _hitCountBeforeKnockBack = 0;
            }

            SoundService.PlaySfx(sfxOnGetHit.PeekRandom(), transform.position);
            _health--;
            monsterHUD.MonsterTakeDamage(1);
            CreateParticleOnHit();
            _animator.SetTrigger(AttackAnims.PeekRandom());

            if (_health <= 0) Die();
        }
    }

    private readonly float _hitCoolTime = 1f;
    private Audio _sing;
    private Audio _talk;

    private void CreateParticleOnHit()
    {
        var particle = Instantiate(particlesOnHit.PeekRandom());
        particle.transform.position = transform.position;
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        Destroy(particle, 5f);
    }

    private void CreateParticleOnDie()
    {
        particlesOnDie.ForEach(p => { p.SetActive(true); });
    }

    private void Die()
    {
        _aiPath.enabled = false;
        disableOnDie.ForEach(go => go.SetActive(false));
        SoundService.PlaySfx(sfxOnDie.PeekRandom(), transform.position);
        CreateParticleOnDie();
        ChangeState(MonsterState.Die);
        Observable.Timer(TimeSpan.FromSeconds(2.1f)).Subscribe(_ =>
        {
            var fuel = Instantiate(fuelTankPrefab);
            fuel.transform.position = transform.position;
            fuel.SetActive(true);
            Destroy(gameObject);
        }).AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_isKnockBack)
        {
            var playerForward = _playerTransform.transform.forward;
            //var dir = new Vector3(playerForward.x, playerForward.y, playerForward.z);
            transform.position += playerForward.normalized * (Time.deltaTime * knockBackIntensity);
        }
    }

    private void ChangeState(MonsterState targetState)
    {
        _state = targetState;
        StopAllCoroutines();

        //Debug.Log($"[MonumentMonster] Changed state to {_state}");

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

    public void OnPlayerEnterFollowProxy(GameObject _)
    {
        ChangeState(MonsterState.Run);
        _talk = SoundService.PlaySfx(sfxTalk, transform.position);
        _talk.AudioSource.spatialize = true;
        _talk.AudioSource.spread = 360f;
        _talk.AudioSource.spatialBlend = 1f;
    }

    public void OnPlayerExitFollowProxy(GameObject _)
    {
        _talk?.Stop();
        ChangeState(MonsterState.Idle);
        _sing = SoundService.PlaySfx(sfxLaugh, transform.position);
        _sing.AudioSource.spatialize = true;
        _sing.AudioSource.spread = 360f;
        _sing.AudioSource.spatialBlend = 1f;
    }

    private IEnumerator IdleRoutine()
    {
        while (_state == MonsterState.Idle)
        {
            yield return YieldInstructionCache.WaitForSeconds(Random.Range(5, 15));
            if (_state != MonsterState.Idle) yield break;
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