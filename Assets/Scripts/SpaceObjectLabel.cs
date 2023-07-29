using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SpaceObjectLabelType
{
    Planet,
    Asteroid,
    Item,
    Debris
}

public class SpaceObjectLabel : MonoBehaviour
{
    [SerializeField] public SpaceObjectLabelType type;
    private Button _enterButton;
    private Action _onClickEnterButton;

    private void Awake()
    {
        _enterButton = GetComponentInChildren<Button>();
        if (_enterButton != default)
            _enterButton.onClick.AddListener(OnClickEnterButton);
    }

    private void OnClickEnterButton()
    {
        _onClickEnterButton?.Invoke();
    }

    public void Show(string text)
    {
        GetComponent<TextMeshPro>().text = text;
    }

    public void RegisterEnterButtonEvent(Action onClick)
    {
        _onClickEnterButton = onClick;
    }
}