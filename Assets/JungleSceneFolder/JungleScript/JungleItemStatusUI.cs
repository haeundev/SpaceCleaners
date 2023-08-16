using GRASBOCK.XR.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JungleItemStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    // [SerializeField] private Image image;
    [SerializeField] public ItemInfo itemInfo;

    public void SetCount(int count)
    {
        text.text = $"{count}/1";
    }
}