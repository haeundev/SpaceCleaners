using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{

    
    private void Awake()
    {
        TaskManager.Instance.levelUpUI = gameObject;
    }

    
}