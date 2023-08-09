using UnityEngine;

public class DebrisBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DebrisBehaviour]  OnTriggerEnter!  other.gameObject.layer: {other.gameObject.layer}");
        if (other.gameObject.layer == LayerMask.NameToLayer("Gadget"))
        {
            if (other.gameObject.name.Contains("Net"))
            {
                OuterSpaceEvent.Trigger_DebrisCaptured(gameObject);
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