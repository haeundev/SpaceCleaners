using DG.Tweening;
using UnityEngine;

public class HorizontalScaleTweener : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float scaleAmount = 3f;
    [SerializeField] private float scaleExpandDuration = 2f;
    [SerializeField] private float scaleRestoreDuration = 0.5f;
    private Vector3 _originalLineScale;
    private Vector3 _targetLineScale;
    
    private void OnEnable()
    {
        _originalLineScale = targetTransform.localScale;
        _targetLineScale = new Vector3(_originalLineScale.x * scaleAmount, _originalLineScale.y, _originalLineScale.z);
        DoTween();
    }

    private void DoTween()
    {
        targetTransform.DOScale(_targetLineScale, scaleExpandDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            targetTransform.DOScale(_originalLineScale, scaleRestoreDuration).SetEase(Ease.OutBounce);
        });
    }
}
