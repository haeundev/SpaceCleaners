using DevFeatures.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class ClearReset : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(ResetAndRestart);
    }

    private void ResetAndRestart()
    {
        SaveAndLoadManager.Instance.ClearAll();
        Application.Quit();
    }
}