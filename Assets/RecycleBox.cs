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
    [SerializeField] private string sfxCorrectRecycle;
    [SerializeField] private string sfxWrongRecycle;
    private IDisposable _particleOnCorrectDisposable;
    private IDisposable _particleOnWrongDisposable;
    [SerializeField] private float increaseSliderValuePerCorrectRecycle = 1f;
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
        _particleOnCorrectDisposable?.Dispose();
        particleOnCorrect.SetActive(false);
        particleOnCorrect.SetActive(true);
        SoundService.PlaySfx(sfxCorrectRecycle, transform.position);
        slider.value += increaseSliderValuePerCorrectRecycle;
        if (slider.value >= 1f)
        {
            slider.value = 1f;
            OnThisBoxDone();
        }
        _particleOnCorrectDisposable = Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => particleOnCorrect.SetActive(false)).AddTo(this);
    }

    private void OnWrongRecycle()
    {
        _particleOnWrongDisposable?.Dispose();
        particleOnWrong.SetActive(false);
        particleOnWrong.SetActive(true);
        SoundService.PlaySfx(sfxWrongRecycle, transform.position);
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