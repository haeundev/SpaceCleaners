using UnityEngine;

public class SpacecraftSpeedlineBehaviour : MonoBehaviour
{
    private void Awake()
    {
        OuterSpaceEvent.OnBoost += OnBoost;
        gameObject.SetActive(false);
    }

    private void OnBoost(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}