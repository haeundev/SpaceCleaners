using UnityEngine;

public class SpaceshipBoundaryWarning : MonoBehaviour
{
    [SerializeField] private Collider headCollider;
    [SerializeField] private GameObject warningUI;

    private void Awake()
    {
        warningUI.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == headCollider) warningUI.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == headCollider) warningUI.SetActive(false);
    }
}