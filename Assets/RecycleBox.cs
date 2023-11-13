using System;
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

    public bool IsDone { get; set; } = false;
    
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
        if (particleOnCorrect == default)
        {
            Debug.LogError("Why is particle default ?");
            return;
        }
        particleOnCorrect.SetActive(false);
        particleOnCorrect.SetActive(true);
        SoundService.PlaySfx("Assets/Audio/correct-choice-43861.mp3", transform.position);
        if (slider == default)
        {
            Debug.LogError("Why is slider default ?");
            return;
        }
        slider.value += 0.35f;
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
        if (particleOnWrong == default)
        {
            Debug.LogError("Why is particleOnWrong default ?");
            return;
        }
        particleOnWrong.SetActive(false);
        particleOnWrong.SetActive(true);
        SoundService.PlaySfx("Assets/Audio/negative_beeps-6008.mp3", transform.position);
        if (slider == default)
        {
            Debug.LogError("Why is slider default ?");
            return;
        }
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
        RecycleManager.Instance.doneRecyclables.Add(recyclableType);
        RecycleManager.Instance.OnOneBoxDone();
    }
}