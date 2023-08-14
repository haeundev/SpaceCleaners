using UnityEngine;
using UnityEngine.UI;

public class DebugCompleteTaskUI : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnPressButton);
    }

    private void OnPressButton()
    {
        TaskManager.Instance.CompleteCurrentTask();
    }
}