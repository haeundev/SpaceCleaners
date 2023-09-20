using System;
using System.Collections.Generic;
using System.Linq;
using LiveLarson.Util;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JungleCompletionChecker : MonoBehaviour
{
    [SerializeField] private List<InsertItemToSlot> inserters;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private List<GameObject> enableOnComplete;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Animator alienAnimator;
    private static readonly int Victory = Animator.StringToHash("Victory");

    private List<OnTriggerWatered> _plants;
    private ColorAdjustments _colorAdjustments;

    private void Awake()
    {
        _plants = FindObjectsOfType<OnTriggerWatered>().ToList();
    }

    public void CheckIfComplete()
    {
        if (inserters.TrueForAll(p => p.isInserted)) OnTaskComplete();
    }

    private void OnTaskComplete()
    {
        enableOnComplete.ForEach(p => p.gameObject.SetActive(true));

        ColorAdjustments colorAdjustments;
        if (globalVolume.profile.TryGet(out colorAdjustments))
            _colorAdjustments = colorAdjustments;
        _colorAdjustments.hueShift.value = 0f;
        
        _plants.Where(p => p.isFullyGrown == false).ForEach(q => q.WaitAndTriggerPlantDone());

        Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
        {
            enableOnComplete.ForEach(p => p.gameObject.SetActive(false));
            JungleEvents.Trigger_SceneComplete();
            instructionText.text = "정글행성 코스 완료!\n다시 우주로 돌아가세요.";
            alienAnimator.SetTrigger(Victory);
        }).AddTo(this);
    }
}