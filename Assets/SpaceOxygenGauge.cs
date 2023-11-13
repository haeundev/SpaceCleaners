using UnityEngine;
using UnityEngine.UI;

public class SpaceOxygenGauge : MonoBehaviour
{
    [SerializeField] private Image additionalOxygen;

    private void OnEnable()
    {
        if (TaskManager.Instance == default)
        {
            return;
        }
        
        if (TaskManager.Instance.CurrentTask.ID >= 8)
            additionalOxygen.color = Color.white;
    }
}