using LiveLarson.SoundSystem;
using TMPro;
using UnityEngine;

public class InstructionUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro description;
    
    private void Awake()
    {
        TaskManager.Instance.instructionUI = gameObject;
    }

    public void SetText(string titleText, string descText)
    {
        title.text = titleText;
        description.text = descText;
        SoundService.PlaySfx("Assets/Audio/Message Appear.ogg", transform.position);
    }
}
