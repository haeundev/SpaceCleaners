using UnityEngine;

public class TutorialArrows : MonoBehaviour
{
    public static TutorialArrows Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public static void Activate(bool isActivate)
    {
        Instance.gameObject.SetActive(isActivate);
    }
}