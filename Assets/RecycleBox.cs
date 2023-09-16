using System;
using DataTables;
using LiveLarson.DataTableManagement;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RecycleBox : MonoBehaviour
{
    [SerializeField] private RecyclableType recyclableType;
    [SerializeField] private GameObject particleOnCorrect;
    [SerializeField] private GameObject particleOnWrong;
    [SerializeField] private Slider slider;
    private IDisposable _particleOnCorrectDisposable;
    private IDisposable _particleOnWrongDisposable;
    private GameConst.DataClass _gameConst;

    public bool IsDone { get; set; } = false;

    private void Awake()
    {
        _gameConst = DataTableManager.GameConst.Data;
    }

    private void Start()
    {
        RecycleManager.Instance.RegisterRecycleBox(this);
        particleOnCorrect.SetActive(false);
        particleOnWrong.SetActive(false);
        slider.value = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Recyclable recyclable))
            return;

        if (recyclable.RecyclableType == recyclableType)
        {
            OnCorrectRecycle();
            Destroy(other.gameObject);
        }
        else
        {
            OnWrongRecycle();
            Destroy(other.gameObject);
        }
    }

    private void OnCorrectRecycle()
    {
        MonumentEvents.Trigger_RecycleCorrect();
        _particleOnCorrectDisposable?.Dispose();
        particleOnCorrect.SetActive(false);
        particleOnCorrect.SetActive(true);
        SoundService.PlaySfx(_gameConst.sfxCorrectRecycle, transform.position);
        slider.value += _gameConst.increaseSliderValuePerCorrectRecycle;
        if (slider.value >= 1f)
        {
            slider.value = 1f;
            OnThisBoxDone();
        }
        _particleOnCorrectDisposable = Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => particleOnCorrect.SetActive(false)).AddTo(this);
    }

    private void OnWrongRecycle()
    {
        MonumentEvents.Trigger_RecycleWrong();
        _particleOnWrongDisposable?.Dispose();
        particleOnWrong.SetActive(false);
        particleOnWrong.SetActive(true);
        SoundService.PlaySfx(_gameConst.sfxWrongRecycle, transform.position);
        slider.value -= 0.1f;
        if (slider.value < 0f)
        {
            slider.value = 0f;
        }
        _particleOnWrongDisposable = Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => particleOnWrong.SetActive(false)).AddTo(this);
    }
    
    private void OnThisBoxDone()
    {
        IsDone = true;
        Debug.Log("ThisBoxDone");
        RecycleManager.Instance.OnOneBoxDone();
    }
}