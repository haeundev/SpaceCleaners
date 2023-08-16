using System.Collections.Generic;
using LiveLarson.BootAndLoad;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")] public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")] public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;

    public List<Button> returnButtons;

    private void Start()
    {
        EnableMainMenu();

        //Hook events
        if (startButton != default)
            startButton.onClick.AddListener(StartGame);
        if (optionButton != default)
            optionButton.onClick.AddListener(EnableOption);
        if (aboutButton != default)
            aboutButton.onClick.AddListener(EnableAbout);
        if (quitButton != default)
            quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons) item.onClick.AddListener(EnableMainMenu);
    }

    private bool isStartedGame;

    // private void Update()
    // {
    //     if (isStartedGame == false)
    //     {
    //         isStartedGame = true;
    //         StartGame();
    //     }
    // }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        ApplicationContext.Instance.LoadScene("OpeningCutscene");

        // if (SaveAndLoadManager.Instance.IsFirstTime)
        //     ApplicationContext.Instance.LoadScene("OpeningCutscene");
        // else
        //     LoadSceneThatMatchTaskID();
    }

    // private void LoadSceneThatMatchTaskID()
    // {
    //     var currentTaskID = SaveAndLoadManager.Instance.GameStat.currentTaskID;
    //     if (currentTaskID == 7 || currentTaskID == 8) // jungle
    //         ApplicationContext.Instance.LoadScene("JunglePlanet");
    //     else if (currentTaskID == 12 || currentTaskID == 13)
    //         ApplicationContext.Instance.LoadScene("MonumentPlanet");
    //     else
    //         ApplicationContext.Instance.LoadScene("OuterSpace");
    // }

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