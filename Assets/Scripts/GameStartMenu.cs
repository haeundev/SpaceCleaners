using System.Collections.Generic;
using DevFeatures.SaveSystem;
using LiveLarson.BootAndLoad;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    private string sceneToGoOnStart = "OuterSpace"; // SCENE NAME
    private string sceneToGoOnStartIfFirst = "OpeningCutscene"; // SCENE NAME
    
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;

    public List<Button> returnButtons;

    private void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    private bool isStartedGame;
    private void Update()
    {
        if (isStartedGame == false )
        {
            isStartedGame = true;
            StartGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        if (SaveAndLoadManager.Instance.IsFirstTime)
        {
            ApplicationContext.Instance.LoadScene(sceneToGoOnStartIfFirst);
        }
        else         
            ApplicationContext.Instance.LoadScene(sceneToGoOnStart);

    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
}
