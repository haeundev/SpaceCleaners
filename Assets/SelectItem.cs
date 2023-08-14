using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectItem : MonoBehaviour
{
    
    [SerializeField] private AudioClip clip;
    
    public InputActionProperty selectItemLeftAction;
    public InputActionProperty selectItemRightAction;
    // Start is called before the first frame update
    private void Awake()
    {
        selectItemLeftAction.action.started += ItemSelected;
        selectItemRightAction.action.started += ItemSelected;
    }

    private void OnDestroy()
    {
        selectItemLeftAction.action.started -= ItemSelected;
        selectItemRightAction.action.started -= ItemSelected;
    }

    private void ItemSelected(InputAction.CallbackContext context)
    {
        AudiosourceManager.instance.PlayClip(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
