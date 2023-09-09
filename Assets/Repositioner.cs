using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class Repositioner : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform origin;
    [SerializeField] private InputActionReference recenterButton;

    private void Start()
    {
        Reposition();
    }

    private void Update()
    {
        if (recenterButton.action.WasPressedThisFrame()) Reposition();
    }

    [Button]
    public void Reposition()
    {
        target.position = origin.position;
        target.forward = origin.forward;
    }
}