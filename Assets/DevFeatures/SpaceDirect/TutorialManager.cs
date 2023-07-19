using System;
using System.Collections;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.InputSystem;

// tutorial direct를 생성해서 디렉터에 넘겨주는 역할
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public InputActionReference button;
    public event Action<int> OnDone;
    private TutorialDirect _tutorialDirect;
    private int _id;

    private void Awake()
    {
        Instance = this;
        _tutorialDirect = new TutorialDirect();
        OnDone += _tutorialDirect.OnDone;
    }
    
    // 외부에서 특정 아이디의 튜토리얼 direct를 예약
    public static void Reserve(int id)
    {
        Instance._tutorialDirect.Reserve(id);
    }

    public static void Run(int id)
    {
        Instance.StartCoroutine(Instance.RunTutorial(id));
    }

    private IEnumerator RunTutorial(int id)
    {
        OpenUI(id);
        
        yield return YieldInstructionCache.WaitUntil(() => _inputTriggered);
        
        // 인풋이 들어온 시점
        OnDone?.Invoke(id);
    }

    private void OpenUI(int id)
    {
        
    }

    private bool _inputTriggered;
    private void Update() // --> 코루틴으로 바꾸면 좋긴 함.
    {
        if (button.action.WasPressedThisFrame())
        {
            _inputTriggered = true;
        }
    }
}
