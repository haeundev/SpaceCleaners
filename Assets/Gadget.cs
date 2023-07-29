using UnityEngine;

public abstract class Gadget : MonoBehaviour
{
    [SerializeField] public int gadgetID;
    [SerializeField] public Transform targetTransform;
    [HideInInspector] public bool isEquipped = true;
    
    public virtual void Init()
    {
        
    }

    public virtual void Use(GameObject targetDebris)
    {
        isEquipped = false;
    }
}