using LiveLarson.BootAndLoad;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToSpaceUI : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.onClick.AddListener(OnPressButton);
    }
    
    private void OnPressButton()
    {
        ApplicationContext.Instance.LoadScene("OuterSpace"); // SCENE NAME
    }
}