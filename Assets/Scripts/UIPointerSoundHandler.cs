using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerSoundHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private enum ButtonType
    {
        Default,
        Confirm,
    }
    
    //private const string SFX_SELECT = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI OK.ogg";
    private string sfxConfirm = "Assets/Audio/Confirm.ogg";
    private string sfxSelect = "Assets/Audio/UI Click.ogg";
    private string sfxHoverEnter = "Assets/Audio/UI Hover Enter.ogg";
    // private const string SFX_HOVER_EXIT = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI Hover Exit.ogg";
    [SerializeField] private ButtonType buttonType;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundService.PlaySfx(buttonType == ButtonType.Confirm ? sfxConfirm : sfxSelect, transform.position);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundService.PlaySfx(sfxHoverEnter, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // SoundService.PlaySfx(SFX_HOVER_EXIT, transform.position);
    }
}