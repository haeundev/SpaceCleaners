using LiveLarson.SoundSystem;
using UnityEngine;

public class SpaceshipBoundaryWarning : MonoBehaviour
{
    [SerializeField] private Collider headCollider;
    [SerializeField] private GameObject warningUI;
    [SerializeField] private string warningSFX = "Assets/Audio/Warning.mp3";
    private Audio _sfx;

    private void Awake()
    {
        warningUI.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == headCollider)
        {
            warningUI.SetActive(true);
            _sfx = SoundService.PlaySfx(warningSFX, transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == headCollider)
        {
            warningUI.SetActive(false);
            _sfx?.Stop();
        }
    }
}