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
    private const string SFX_CONFIRM = "Audio/Confirm.ogg";
    private const string SFX_SELECT = "Audio/UI Click.ogg";
    private const string SFX_HOVER_ENTER = "Audio/UI Hover Enter.ogg";
    // private const string SFX_HOVER_EXIT = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI Hover Exit.ogg";

    [SerializeField] private ButtonType buttonType;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundService.PlaySfx(buttonType == ButtonType.Confirm ? SFX_CONFIRM : SFX_SELECT, transform.position);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundService.PlaySfx(SFX_HOVER_ENTER, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // SoundService.PlaySfx(SFX_HOVER_EXIT, transform.position);
    }
}