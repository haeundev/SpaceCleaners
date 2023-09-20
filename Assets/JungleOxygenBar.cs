using GRASBOCK.XR.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class JungleOxygenBar : MonoBehaviour
{
    private Slider _slider;
    private const int TotalItemCount = 3;
    private int _currentItemCount;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        JungleEvents.OnInventoryUpdated += OnInventoryUpdated;
    }

    private void Start()
    {
        _slider.value = 0f;
    }

    private void OnInventoryUpdated(Slot _)
    {
        Debug.Log("OnInventoryUpdated");
        _currentItemCount++;

        if (_currentItemCount == TotalItemCount)
            _slider.value = 1f;
        else
            _slider.value = _currentItemCount * 0.3f;
    }
}