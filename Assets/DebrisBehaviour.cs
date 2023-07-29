using UnityEngine;

public class DebrisBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Gadget"))
        {
            if (other.gameObject.name.Contains("Net"))
            {
                Destroy(GetComponentInChildren<Collider>());
                Destroy(other.GetComponentInChildren<Rigidbody>());
                Destroy(other.GetComponentInChildren<Collider>());
                OuterSpaceEvent.Trigger_DebrisCaptured(gameObject);
                transform.SetParent(other.transform);
                transform.localPosition = Vector3.zero;
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