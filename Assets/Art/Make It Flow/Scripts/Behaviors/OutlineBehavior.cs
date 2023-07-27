using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MeadowGames.MakeItFlow;

public class OutlineBehavior : Behavior
{
    public Color color = Color.yellow;
    public float width = 8;

    Dictionary<MFObject, Outline> dictMFObjToAct = new Dictionary<MFObject, Outline>();

    public override void InitializeBehavior()
    {
        dictMFObjToAct.Clear();
        foreach (MFObject objAct in MFObjectsToAct)
        {
            GameObject objActGO = objAct.Transform.gameObject;
            Outline outline = objActGO.GetComponent<Outline>();
            if (!outline)
            {
                outline = objActGO.AddComponent<Outline>();
                outline.enabled = false;
            }

            // outline.effectColor = color; //일단은 주석처리
            // outline.effectDistance = new Vector2(width, width);

            dictMFObjToAct.Add(objAct, outline);
        }
    }

    void OnValidate()
    {
        foreach (var item in dictMFObjToAct)
        {
            // item.Value.effectColor = color; //일단은 주석처리
            // item.Value.effectDistance = new Vector2(width, width);
        }
    }

    public override void StartBehavior()
    {
        behaviorEvents.OnBehaviorStart.Invoke();
        foreach (var item in dictMFObjToAct)
        {
            if (item.Value) item.Value.enabled = true;
        }
    }

    public override void InterruptBehavior()
    {
        foreach (var item in dictMFObjToAct)
        {
            if (item.Value) item.Value.enabled = false;
        }
        behaviorEvents.OnBehaviorInterrupt.Invoke();
    }
}
