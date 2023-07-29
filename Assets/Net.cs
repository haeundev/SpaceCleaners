using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class Net : Gadget
{
    [SerializeField] private Transform netHead;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float captureTimingRatioInAnimClip = 0.45f;
    [SerializeField] private Vector3 distanceToSetOnCapturing;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private bool _isCaptured;
    private Animator _animator;
    private Transform _netCapturePositionObj;

    private void Awake()
    {
        _netCapturePositionObj = GameObject.Find("NetCapturePosition").transform;
    }
    // for test
    // private void Start()
    // {
    //     transform.LookAt(targetTransform);
    //     transform.position = netHead.position + offset;
    // }
    
    [Button]
    public override void Init()
    {
        if (_isCaptured)
            return;
        
        base.Init();
        _animator = GetComponentInChildren<Animator>();
        transform.LookAt(targetTransform);
        transform.position = netHead.position + offset;
        OuterSpaceEvent.OnGadgetShoot += Use;
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
    }

    [Button]
    private void OnDebrisCaptured(GameObject targetDebris)
    {
        if (_isCaptured)
            return;
        
        _isCaptured = true;
        var currentClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        _animator.Play(currentClip.name, 0, captureTimingRatioInAnimClip);
        Observable.Timer(TimeSpan.FromSeconds(currentClip.length * (1 - captureTimingRatioInAnimClip))).Subscribe(_ =>
        {
            Destroy(gameObject);
            Destroy(targetDebris);
        }).AddTo(this);
        
        transform.position = _netCapturePositionObj.transform.position;
        //
        // // Step 3: Make the player face the enemy
        // Vector3 lookAtPosition = new Vector3(debrisTransform.position.x, transform.position.y, debrisTransform.position.z);
        // transform.LookAt(lookAtPosition);
    }

    [Button]
    public override void Use(GameObject targetDebris)
    {
        if (_isCaptured)
            return;
        
        base.Use(targetDebris);
        transform.LookAt(targetDebris.transform);
        _animator.SetTrigger(Shoot);
    }
}