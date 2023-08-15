using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerActionHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<GameObject> activateOnHover;
    [SerializeField] private List<GameObject> deactivateOnHover;
    [SerializeField] private List<GameObject> deactivateOnHoverEnd;
    [SerializeField] private List<GameObject> activateOnSelectIfOpen;
    [SerializeField] private List<GameObject> deactivateOnSelectIfOpen;
    [SerializeField] private List<GameObject> activateOnSelectIfClosed;
    [SerializeField] private List<GameObject> deactivateOnSelect;
    [SerializeField] private bool deactivateSelfOnSelect;
    private bool _isOpen;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {gameObject.name}");
        if (_isOpen)
        {
            activateOnSelectIfOpen.ForEach(p => p.SetActive(true));
            deactivateOnSelectIfOpen.ForEach(p => p.SetActive(false));
        }
        else
        {
            activateOnSelectIfClosed.ForEach(p => p.SetActive(true));
        }
        deactivateOnSelect.ForEach(p => p.SetActive(false));
        if (deactivateSelfOnSelect)
            gameObject.SetActive(false);
        _isOpen = !_isOpen;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        activateOnHover.ForEach(p => p.SetActive(true));
        deactivateOnHover.ForEach(p => p.SetActive(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        activateOnHover.ForEach(p => p.SetActive(false));
        deactivateOnHoverEnd.ForEach(p => p.SetActive(false));
        deactivateOnHover.ForEach(p => p.SetActive(true));
    }
}