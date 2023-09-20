using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class JungleCompletionChecker : MonoBehaviour
{
    [SerializeField] private List<InsertItemToSlot> inserters;
    [SerializeField] private TextMeshProUGUI instructionText;

    public void CheckIfComplete()
    {
        if (inserters.TrueForAll(p => p.isInserted))
        {
            Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
            {
                Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ =>
                {
                    JungleEvents.Trigger_SceneComplete();
                    instructionText.text = "정글행성 코스 완료!\n다시 우주로 돌아가세요.";
                }).AddTo(this);
            }).AddTo(this);
        }
    }
}