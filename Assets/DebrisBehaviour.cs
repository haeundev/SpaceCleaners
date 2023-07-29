using UnityEngine;

public class DebrisBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Gadget"))
        {
            Destroy(GetComponentInChildren<Collider>());
            Destroy(other.GetComponentInChildren<Rigidbody>());
            Destroy(other.GetComponentInChildren<Collider>());
            
            OuterSpaceEvent.Trigger_DebrisCaptured(gameObject);
            transform.SetParent(other.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}