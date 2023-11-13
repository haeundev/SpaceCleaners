using TMPro;
using UnityEngine;

public class MonumentCompletionChecker : MonoBehaviour
{
    [SerializeField] private TextMeshPro instructionText;

    private void Awake()
    {
        MonumentEvents.OnSceneComplete += OnComplete;
    }

    private void OnComplete()
    {
        instructionText.text = "모래행성 코스 완료!\n다시 우주로 돌아가세요.";
    }

    private void OnDestroy()
    {
        MonumentEvents.OnSceneComplete -= OnComplete;
    }
}