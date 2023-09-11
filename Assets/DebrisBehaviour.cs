using UnityEngine;

public enum DebrisType
{
    Turbine,
    Satellite
}

public class DebrisBehaviour : MonoBehaviour
{
    [SerializeField] private DebrisType debrisType;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DebrisBehaviour]  OnTriggerEnter!  other.gameObject.layer: {other.gameObject.layer}");
        if (other.gameObject.layer == LayerMask.NameToLayer("Gadget"))
        {
            if (other.gameObject.name.Contains("Net"))
            {
                OuterSpaceEvent.Trigger_DebrisCaptured(debrisType, gameObject);
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
            else if (other.gameObject.name.Contains("Grapple"))
            {
                
            }
            else if (other.gameObject.name.Contains("fff"))
            {
                
            }
        }
    }
}