using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevFeatures.SaveSystem;
using LiveLarson.BootAndLoad;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    private List<SingleCutBehaviour> _cuts;
    private Queue<SingleCutBehaviour> _cutQueue;

    private void Awake()
    {
        SaveAndLoadManager.Instance.GameStat.isWatchedOpeningCutscene = true;
        SaveAndLoadManager.Instance.Save(SaveAndLoadManager.Instance.GameStat);
        
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
        Debug.Log("000");

        yield return YieldInstructionCache.WaitForSeconds(5f);

        Debug.Log("111");

        while (_cutQueue.Count > 0)
        {
            var cut = _cutQueue.Dequeue();
            cut.gameObject.SetActive(true);
            SoundService.PlaySfx(cut.audioPath, default);
            yield return YieldInstructionCache.WaitForSeconds(cut.duration);
            Debug.Log("222");
            cut.gameObject.SetActive(false);
        }
        
        ApplicationContext.Instance.LoadScene("OuterSpace"); // SCENE NAME
    }
}