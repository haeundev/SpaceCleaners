using LiveLarson.Enums;
using LiveLarson.GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Booter : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Booting_1_ApplicationContext";
    [SerializeField] private GameMode nextGameMode;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameModeService.Instance.TryEnterGameMode(nextGameMode);
        SceneManager.LoadScene(nextSceneName);
    }
}