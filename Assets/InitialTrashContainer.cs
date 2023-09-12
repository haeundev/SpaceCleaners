using System.Collections;
using System.Collections.Generic;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InitialTrashContainer : MonoBehaviour
{
    [Button]
    public void Initialize()
    {
        GetComponentsInChildren<XRGrabInteractable>().ForEach(DestroyImmediate);
        GetComponentsInChildren<Recyclable>().ForEach(DestroyImmediate);
        GetComponentsInChildren<Collider>().ForEach(DestroyImmediate);
        GetComponentsInChildren<Rigidbody>().ForEach(DestroyImmediate);
    }
}
