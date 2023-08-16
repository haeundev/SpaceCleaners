using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectItem : MonoBehaviour
{
    
    [SerializeField] private AudioClip enteringClip;
    [SerializeField] private AudioClip exitingClip;
    
    // public InputActionProperty selectItemLeftAction;
    // public InputActionProperty selectItemRightAction;
    // Start is called before the first frame update

    public XRGrabInteractable grabInteractable;

    public bool isFinalObject = false;
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        

        grabInteractable.selectEntered.AddListener(ItemSelected);
        grabInteractable.selectExited.AddListener(ItemUnSelected);
        // selectItemLeftAction.action.started += ItemSelected;
        // selectItemRightAction.action.started += ItemSelected;
    }

    private void ItemSelected(SelectEnterEventArgs context)
    {
        AudiosourceManager.instance.PlayClip(enteringClip);
    }
    
    private void ItemUnSelected(SelectExitEventArgs context)
    {
        AudiosourceManager.instance.PlayClip(exitingClip);

        if (isFinalObject)
        {
            //여기서 미션 컴플릿
            JungleEvents.Trigger_SceneComplete();
        }
    }
}
