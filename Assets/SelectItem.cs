using System.Collections;
using System.Collections.Generic;
using LiveLarson.Enums;
using LiveLarson.GameMode;
using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
        if (AudiosourceManager.instance == default)
        {
            SoundService.PlaySfx("Assets/Audio/Grabbing.mp3", transform.position);
        }
        else
        {
            AudiosourceManager.instance.PlayClip(enteringClip);
        }
    }
    
    private void ItemUnSelected(SelectExitEventArgs context)
    {
        if (AudiosourceManager.instance == default)
        {
            SoundService.PlaySfx("Assets/Audio/Grabbing.mp3", transform.position);
        }
        else
        {
            AudiosourceManager.instance.PlayClip(exitingClip);
        }
        
        if (isFinalObject)
        {
            print(TaskManager.Instance.CurrentTask.TaskType);
            // planet mission complete
            if (TaskManager.Instance.CurrentTask.TaskType == TaskType.MissionOnPlanet)
            {
                if (SceneManager.GetActiveScene().name.Contains("Jungle"))
                {
                    JungleEvents.Trigger_SceneComplete();
                }
                if (SceneManager.GetActiveScene().name.Contains("Monument"))
                {
                    MonumentEvents.Trigger_SceneComplete();
                }
            }
           
        }
    }
}
