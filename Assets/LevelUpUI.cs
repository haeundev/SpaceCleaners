using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    private void Awake()
    {
        TaskManager.Instance.levelUpUI = gameObject;
    }
}