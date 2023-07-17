using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Main Camera 밑에 배치하고 Fader Screen 참조.
public class SceneTransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public static SceneTransitionManager Instance;

    private void Awake()
    {
        if (Instance && Instance != this)
            Destroy(Instance);

        Instance = this;
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(GoToSceneRoutine(sceneName));
    }

    private IEnumerator GoToSceneRoutine(string sceneName)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //Launch the new scene
        SceneManager.LoadScene(sceneName);
    }

    public void GoToSceneAsync(string sceneName)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneName));
    }

    private IEnumerator GoToSceneAsyncRoutine(string sceneName)
    {
        fadeScreen.FadeOut();
        //Launch the new scene
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0;
        while (timer <= fadeScreen.fadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        operation.allowSceneActivation = true;
    }
}