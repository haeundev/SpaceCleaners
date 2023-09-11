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
    [SerializeField] private GameObject turbineModel;
    [SerializeField] private GameObject satelliteModel;

    private void Awake()
    {
        _netCapturePositionObj = GameObject.Find("NetCapturePosition").transform;
    }

    private void Start()
    {
        TurnOffModels();
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

    private void OnDestroy()
    {
        OuterSpaceEvent.OnGadgetShoot -= Use;
        OuterSpaceEvent.OnDebrisCaptured -= OnDebrisCaptured;
    }

    [Button]
    private void OnDebrisCaptured(DebrisType debrisType, GameObject targetDebris)
    {
        if (_isCaptured)
            return;
        
        _isCaptured = true;
        
        // reset
        TurnOffModels();
        
        switch (debrisType)
        {
            case DebrisType.Turbine:
                turbineModel.SetActive(true);
                break;
            case DebrisType.Satellite:
                satelliteModel.SetActive(true);
                break;
        }
        
        var currentClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        _animator.Play(currentClip.name, 0, captureTimingRatioInAnimClip);
        Observable.Timer(TimeSpan.FromSeconds(currentClip.length * (1 - captureTimingRatioInAnimClip))).Subscribe(_ =>
        {
            TurnOffModels();
            Destroy(gameObject);
            Destroy(targetDebris);
        }).AddTo(this);
        
        transform.position = _netCapturePositionObj.transform.position;
    }

    private void TurnOffModels()
    {
        turbineModel.SetActive(false);
        satelliteModel.SetActive(false);
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