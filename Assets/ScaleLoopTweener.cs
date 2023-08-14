using DG.Tweening;
using UnityEngine;

public class ScaleLoopTweener : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private Vector3 _originalScale;
    private Vector3 _scaleTo;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scaleRatio = 1.5f;
    
    private void OnEnable()
    {
        _originalScale = targetTransform.localScale;
        _scaleTo = _originalScale * scaleRatio;
        DoTween();
    }

    private void DoTween()
    {
        targetTransform.DOScale(_scaleTo, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                targetTransform.DOScale(_originalScale, duration).SetEase(Ease.OutBounce);
            });
    }
}