using UnityEngine;

public class SpacecraftSpeedlineBehaviour : MonoBehaviour
{
    private void Awake()
    {
        OuterSpaceEvent.OnBoost += OnBoost;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnBoost -= OnBoost;
    }

    private void OnBoost(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}