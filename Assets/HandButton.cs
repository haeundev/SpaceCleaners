using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HandButton : XRBaseInteractable
{
    public UnityEvent OnPress = null;
    
    private float yMin = 0.0f;
    private float yMax = 0.0f;
    private bool previousPress = false;
    
    private float previousHandHeight = 0.0f;
    private IXRHoverInteractor hoverInteractor = null;

    protected void Awake()
    {
        base.Awake();
        hoverEntered.AddListener(StartPress);
        hoverExited.AddListener(EndPress);
    }

    private void OnDestroy()
    {
        hoverEntered.RemoveListener(StartPress);
        hoverExited.RemoveListener(EndPress);
    }

    private void StartPress(HoverEnterEventArgs ctx)
    {
        hoverInteractor = ctx.interactorObject;
        previousHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
    }

    private void EndPress(HoverExitEventArgs ctx)
    {
        hoverInteractor = null;
        previousHandHeight = 0.0f;

        previousPress = false;
        SetYPosition(yMax);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMinMax();
    }

    private void SetMinMax()
    {
        Collider collider = GetComponent<Collider>();
        yMin = transform.localPosition.y - (collider.bounds.size.y * 0.25f);
        yMax = transform.localPosition.y;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (hoverInteractor!=null)
        {
            float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
            float handDifference = previousHandHeight - newHandHeight;
            previousHandHeight = newHandHeight;

            float newPosition = transform.localPosition.y - handDifference;
            SetYPosition(newPosition);
            
            CheckPress();
        }
    }

    private float GetLocalYPosition(Vector3 position)
    {
        Vector3 localPosition = transform.root.InverseTransformPoint(position);
        return localPosition.y;
    }

    private void SetYPosition(float position)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = Mathf.Clamp(position, yMin, yMax);
        transform.localPosition = newPosition;
    }

    private void CheckPress()
    {
        bool inPosition = InPosition();

        if (inPosition && inPosition != previousPress)
        {
            OnPress.Invoke();
        }

        previousPress = inPosition;
    }

    private bool InPosition()
    {
        float inRange = Mathf.Clamp(transform.localPosition.y, yMin, yMin + 0.01f);
        
        return transform.localPosition.y == inRange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
