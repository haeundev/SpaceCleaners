using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiveLarson.BootAndLoad;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UnityEngine;

public enum CutsceneType
{
    Opening,
    Ending
}

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private CutsceneType cutsceneType;
    private List<SingleCutBehaviour> _cuts;
    private Queue<SingleCutBehaviour> _cutQueue;

    private void Awake()
    {
        //SaveAndLoadManager.Instance.GameStat.isWatchedOpeningCutscene = true;
        //SaveAndLoadManager.Instance.Save(SaveAndLoadManager.Instance.GameStat);

        _cuts = FindObjectsOfType<SingleCutBehaviour>().OrderBy(p => p.gameObject.name).ToList();
        _cutQueue = new Queue<SingleCutBehaviour>();
        _cuts.ForEach(p => _cutQueue.Enqueue(p));
        _cuts.ForEach(p => p.gameObject.SetActive(false));
    }

    private void Start()
    {
        StartCoroutine(PlayCuts());
    }

    private IEnumerator PlayCuts()
    {
        yield return YieldInstructionCache.WaitForSeconds(5f);

        while (_cutQueue.Count > 0)
        {
            var cut = _cutQueue.Dequeue();
            cut.gameObject.SetActive(true);
            // if (string.IsNullOrEmpty(cut.audioPath) == false) SoundService.PlaySfx(cut.audioPath, default);
            yield return YieldInstructionCache.WaitForSeconds(cut.duration);
            cut.gameObject.SetActive(false);
        }

        FinishCutscene();
    }

    [Button]
    private void FinishCutscene()
    {
        if (ApplicationContext.Instance == default)
        {
            Debug.LogError($"Please start with the first booting scene");
            return;
        }

        switch (cutsceneType)
        {
            case CutsceneType.Opening:
                ApplicationContext.Instance.LoadScene("OuterSpace"); // SCENE NAME
                break;
            case CutsceneType.Ending:
                ApplicationContext.Instance.LoadScene("CreditScene"); // SCENE NAME
                break;
        }
    }
}