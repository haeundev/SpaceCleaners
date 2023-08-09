using DG.Tweening;
using UnityEngine;

public class RandomMoveRotateTween : MonoBehaviour
{
    public float moveDistance = 100f;
    public float moveDuration = 60f;
    public float rotateDuration = 60f;

    private void Start()
    {
        MoveAndRotateRandomly();
    }

    private void MoveAndRotateRandomly()
    {
        // Random movement
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * moveDistance;
        transform.DOMove(randomPosition, moveDuration).SetEase(Ease.InOutSine).OnComplete(MoveAndRotateRandomly);

        // Random rotation
        Vector3 randomRotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        transform.DORotate(randomRotation, rotateDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine)
            .OnComplete(MoveAndRotateRandomly);
    }
}