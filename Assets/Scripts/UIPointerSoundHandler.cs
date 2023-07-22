using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerSoundHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private const string SFX_SELECT = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI OK.ogg";
    private const string SFX_HOVER_ENTER = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI Select.ogg";
    private const string SFX_HOVER_EXIT = "Audio/Kenney Audio/kenney_interface-sounds/Audio/UI Hover Exit.ogg";

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundService.PlaySfx(SFX_SELECT, default);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundService.PlaySfx(SFX_HOVER_ENTER, default);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SoundService.PlaySfx(SFX_HOVER_EXIT, default);
    }
}