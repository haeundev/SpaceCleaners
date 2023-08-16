using UnityEngine;
using UnityEngine.SceneManagement;

public class Booter : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Booting_1_ApplicationContext";
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}